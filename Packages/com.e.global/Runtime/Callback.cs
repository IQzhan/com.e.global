using System;
using System.Threading;
using Unity.Collections;

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

    internal class CallbackBase<T> : IDisposable where T : unmanaged
    {
        private readonly uint m_ID = Utility.UniqueGlobalID();

        internal NativeHashMap<CallbackHandle, T> all = new NativeHashMap<CallbackHandle, T>(4, Allocator.Persistent);

        private int m_NativeIndex = 0;

        private bool m_DisposedValue;

        internal CallbackHandle InternalAdd(T callback)
        {
            Interlocked.Add(ref m_NativeIndex, 1);
            CallbackHandle key = new CallbackHandle() { from = m_ID, index = (uint)m_NativeIndex };
            all.TryAdd(key, callback);
            return key;
        }

        internal void InternalRemove(CallbackHandle handle)
        {
            if (handle.from != m_ID) return;
            all.Remove(handle);
        }

        protected virtual void InnerDispose()
        {
            if (!m_DisposedValue)
            {
                if (all.IsCreated)
                {
                    all.Dispose();
                }
                m_DisposedValue = true;
            }
        }

        ~CallbackBase()
        {
            InnerDispose();
        }

        public void Dispose()
        {
            InnerDispose();
            GC.SuppressFinalize(this);
        }
    }

    public class Callback<T>
        where T : unmanaged, ICallback
    {
        private readonly CallbackBase<T> callbacks = new CallbackBase<T>();

        public CallbackHandle Add(T callback)
        { return callbacks.InternalAdd(callback); }

        public void Remove(CallbackHandle handle)
        { callbacks.InternalRemove(handle); }

        public void Invoke()
        {
            foreach(var kv in callbacks.all)
            {
                kv.Value.Invoke();
            }
        }

        public void InvokeJob()
        {

        }
    }
}