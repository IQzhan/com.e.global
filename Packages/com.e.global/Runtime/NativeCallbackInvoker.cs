using System;
using System.Runtime.InteropServices;
using System.Threading;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace E
{
    public struct CallbackHandle : IEquatable<CallbackHandle>
    {
        internal uint from;

        internal uint index;

        public ulong ID { get => (from << 32) | index; }

        public override bool Equals(object obj)
        {
            return obj is CallbackHandle handle && Equals(handle);
        }

        public bool Equals(CallbackHandle other)
        {
            return from == other.from &&
                   index == other.index;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public static bool operator ==(CallbackHandle left, CallbackHandle right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CallbackHandle left, CallbackHandle right)
        {
            return !(left == right);
        }
    }

    internal interface IInvoker<TCallback> where TCallback : struct, ICallback
    { public void Invoke(TCallback callback); }

    public struct NativeAction<TCallback, T0, T1, T2> : IDisposable
        where TCallback : unmanaged, IAction<T0, T1, T2>
        where T0 : unmanaged where T1 : unmanaged where T2 : unmanaged
    {
        private NativeCallbackInvoker<TCallback> m_NativeInvoker;
        public NativeAction(Allocator allocator) { m_NativeInvoker = new NativeCallbackInvoker<TCallback>(allocator); }
        private struct ActionInvoker : IInvoker<TCallback>
        {
            public T0 t0; public T1 t1; public T2 t2;
            public void Invoke(TCallback callback) { callback.Invoke(t0, t1, t2); }
        }
        public CallbackHandle Add(TCallback callback) { return m_NativeInvoker.Add(callback); }
        public void Remove(CallbackHandle handle) { m_NativeInvoker.Remove(handle); }
        public void Clear() { m_NativeInvoker.Clear(); }
        public JobHandle InvokeJob<TInvoker>(T0 t0, T1 t1, T2 t2, JobHandle dependsOn = default)
        { return m_NativeInvoker.InvokeJob(new ActionInvoker { t0 = t0, t1 = t1, t2 = t2 }, dependsOn); }
        public JobHandle InvokeJobParallel<TInvoker>(T0 t0, T1 t1, T2 t2, JobHandle dependsOn = default)
        { return m_NativeInvoker.InvokeJobParallel(new ActionInvoker { t0 = t0, t1 = t1, t2 = t2 }, dependsOn); }
        public void Dispose() { m_NativeInvoker.Dispose(); }
    }

    internal unsafe struct NativeCallbackInvoker<TCallback> : IDisposable
        where TCallback : unmanaged, ICallback
    {
        [StructLayout(LayoutKind.Explicit)]
        [BurstCompatible]
        private struct Data
        {
            [FieldOffset(0)]
            public uint id;
            [FieldOffset(4)]
            public int innerID;
        }

        private Data* m_Data;

        private NativeHashMap<CallbackHandle, TCallback> map;

        private Allocator allocator;

        public NativeCallbackInvoker(Allocator allocator)
        {
            this.allocator = allocator;
            m_Data = (Data*)UnsafeUtility.Malloc(sizeof(Data), UnsafeUtility.AlignOf<Data>(), allocator);
            m_Data->id = Utility.UniqueGlobalID();
            m_Data->innerID = 0;
            map = new NativeHashMap<CallbackHandle, TCallback>(4, allocator);
        }

        public void Dispose()
        {
            if (m_Data != null) UnsafeUtility.Free(m_Data, allocator);
            if (map.IsCreated) map.Dispose();
            GC.SuppressFinalize(this);
        }

        public CallbackHandle Add(TCallback callback)
        {
            Interlocked.Add(ref m_Data->innerID, 1);
            CallbackHandle key = new CallbackHandle() { from = m_Data->id, index = (uint)m_Data->innerID };
            map.TryAdd(key, callback);
            return key;
        }

        public void Remove(CallbackHandle handle)
        {
            if (handle.from != m_Data->id) return;
            map.Remove(handle);
        }

        public void Clear() { map.Clear(); }

        public JobHandle InvokeJob<TInvoker>(TInvoker invoker, JobHandle dependsOn = default)
            where TInvoker : unmanaged, IInvoker<TCallback>
        { return new Job<TInvoker>(invoker, map.GetUnsafeBucketData(), map.Count()).Schedule(dependsOn); }

        public JobHandle InvokeJobParallel<TInvoker>(TInvoker invoker, JobHandle dependsOn = default)
            where TInvoker : unmanaged, IInvoker<TCallback>
        {
            int length = map.Count();
            int bacthCount = 32;
            return new JobParallelFor<TInvoker>(invoker, map.GetUnsafeBucketData()).Schedule(length, bacthCount, dependsOn);
        }

        private unsafe struct Job<TInvoker> : IJob
            where TInvoker : unmanaged, IInvoker<TCallback>
        {
            private readonly int* buckets;
            private readonly int* nextPtrs;
            private readonly byte* values;
            private readonly int count;
            private readonly TInvoker invoker;

            public Job(TInvoker invoker, UnsafeHashMapBucketData bucketData, int count)
            {
                buckets = (int*)bucketData.buckets;
                nextPtrs = (int*)bucketData.next;
                values = bucketData.values;
                this.count = count;
                this.invoker = invoker;
            }

            public void Execute()
            {
                for (int index = 0; index < count; index++)
                {
                    int entryIndex = buckets[index];
                    while (entryIndex != -1)
                    {
                        TCallback value = UnsafeUtility.ArrayElementAsRef<TCallback>(values, entryIndex);
                        invoker.Invoke(value);
                        entryIndex = nextPtrs[entryIndex];
                    }
                }
            }
        }

        private unsafe struct JobParallelFor<TInvoker> : IJobParallelFor
            where TInvoker : unmanaged, IInvoker<TCallback>
        {
            private readonly int* buckets;
            private readonly int* nextPtrs;
            private readonly byte* values;
            private readonly TInvoker invoker;

            public JobParallelFor(TInvoker invoker, UnsafeHashMapBucketData bucketData)
            {
                buckets = (int*)bucketData.buckets;
                nextPtrs = (int*)bucketData.next;
                values = bucketData.values;
                this.invoker = invoker;
            }

            public void Execute(int index)
            {
                int entryIndex = buckets[index];
                while (entryIndex != -1)
                {
                    TCallback value = UnsafeUtility.ArrayElementAsRef<TCallback>(values, entryIndex);
                    invoker.Invoke(value);
                    entryIndex = nextPtrs[entryIndex];
                }
            }
        }
    }
}