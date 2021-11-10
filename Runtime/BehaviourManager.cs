﻿using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace E
{
    public sealed partial class BehaviourManager
    {
        public static bool IsReady { get => m_Instance.m_IsReady; }

        public static BehaviourSettings.UpdateMethod UpdateMethod { get => BehaviourSettings.Instance.updateMethod; }

        public static float DeltaTime { get => BehaviourSettings.Instance.deltaTime; }

        public static BehaviourUpdater MonoBehaviour { get => BehaviourUpdater.Instance; }

        public static event Func<IEnumerable<TypeInfo>, IEnumerable<TypeInfo>> ResetTypeInfosCallback;

        public static event Func<TypeInfo, GlobalBehaviour> OverrideCreateInstanceCallback;

        public static event Action FixedUpdateCallback;

        public static event Action UpdateCallback;

        public static event Action LateUpdateCallback;

#if UNITY_EDITOR
        public static event Action OnDrawGizmosCallback;
#endif

        private bool m_IsReady;

        private SortedList<int, TypeInfo> m_TypeInfos;

        private BehaviourCollection m_Collection;

        private static BehaviourManager m_Instance;

        private bool m_IsFirstAccessing;

        private double m_LastTime;

        private List<int> m_EnableQueue;

        private List<int> m_UpdateQueue;

        private List<int> m_DisableQueue;

        private enum StateToCheck
        {
            Enable = 1,
            Update = 1 << 1,
            Disable = 1 << 2,
            All = Enable | Update | Disable,
            UpdateAndDisable = Update | Disable
        }

        static BehaviourManager()
        { m_Instance = new BehaviourManager(); }

        private BehaviourManager() { Initialize(); }

        ~BehaviourManager() { Destroy(); }

        public static T CreateInstance<T>() where T : GlobalBehaviour
        {
            return m_Instance.InternalCreateInstance<T>();
        }

        public static GlobalBehaviour CreateInstance(in Type type)
        {
            return m_Instance.InternalCreateInstance(type);
        }

        public static T GetInstance<T>() where T : GlobalBehaviour
        {
            return m_Instance.InternalGetInstance<T>();
        }

        public static GlobalBehaviour GetInstance(in Type type)
        {
            return m_Instance.InternalGetInstance(type);
        }

        public static T[] GetInstances<T>() where T : GlobalBehaviour
        {
            return m_Instance.InternalGetInstances<T>();
        }

        public static GlobalBehaviour[] GetInstances(in Type type)
        {
            return m_Instance.InternalGetInstances(type);
        }

        public static void DestroyInstance(in GlobalBehaviour behaviour)
        {
            m_Instance.InternalDestroyInstance(behaviour);
        }

#if UNITY_EDITOR
        [InitializeOnLoadMethod, UnityEditor.Callbacks.DidReloadScripts]
#else
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif
        private static void InitializeOnLoadInRuntime()
        {
            m_Instance.FirstAccess();
        }

        private void Initialize()
        {
            try
            {
                if (m_IsReady) return;
                CreateTypeInfos();
                CreateLifeCycleQueues();
                CreateCollection();
                ClearCallbacks();
                BehaviourUpdater.CreateInstance();
                BehaviourUpdater.Instance.manager = this;
                m_IsReady = true;
                m_IsFirstAccessing = true;
            }
            catch (Exception e)
            {
                if (Debug.isDebugBuild)
                {
                    Debug.LogException(e);
                }
                return;
            }
        }

        private void Destroy()
        {
            try
            {
                if (!m_IsReady) return;
                m_IsReady = false;
                BehaviourUpdater.DestroyInstance();
                ReleaseCollection();
                ClearCallbacks();
                ReleaseLifeCycleQueues();
                ReleaseTypeInfos();
                GC.SuppressFinalize(this);
            }
            catch (Exception e)
            {
                if (Debug.isDebugBuild)
                {
                    Debug.LogException(e);
                }
                return;
            }
        }

        private void ExecuteBefore()
        {
            // TODO 查找函数
            // InitializeBeforeAllBehavioursMethodAttribute
            //AppDomain.CurrentDomain.GetAssemblies()
            //    .SelectMany(a => a.GetTypes().SelectMany(t => t.GetMethods()
            //    .Where(m => m.IsStatic)))
        }

        private void CreateTypeInfos()
        {
            var baseType = typeof(GlobalBehaviour);
            IEnumerable<TypeInfo> typeInfos =
                AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes()
                .Where(t => baseType.IsAssignableFrom(t) &&
                t.IsClass &&
                !t.IsAbstract &&
                !t.IsGenericType)
                .Select(t => new TypeInfo(t)));
            if (ResetTypeInfosCallback != null)
            {
                typeInfos = ResetTypeInfosCallback(typeInfos);
            }
            m_TypeInfos = new SortedList<int, TypeInfo>
                (typeInfos.ToDictionary(i => i.type.GetHashCode()));
        }

        private void ReleaseTypeInfos()
        {
            m_TypeInfos.Clear();
            m_TypeInfos = null;
        }

        private void CreateCollection()
        {
            m_Collection = new BehaviourCollection();
        }

        private void ReleaseCollection()
        {
            foreach (GlobalBehaviour behaviour in m_Collection)
            {
                try
                {
                    behaviour.InternalDestroy();
                }
                catch (Exception e)
                {
                    if (Debug.isDebugBuild)
                    {
                        Debug.LogException(e);
                    }
                }
            }
            m_Collection.Clear();
            m_Collection = null;
        }

        private void CreateLifeCycleQueues()
        {
            m_EnableQueue = new List<int>();
            m_UpdateQueue = new List<int>();
            m_DisableQueue = new List<int>();
        }

        private void ReleaseLifeCycleQueues()
        {
            ClearLifeCycleQueues();
            m_EnableQueue = null;
            m_UpdateQueue = null;
            m_DisableQueue = null;
        }

        private void ClearLifeCycleQueues()
        {
            m_EnableQueue.Clear();
            m_UpdateQueue.Clear();
            m_DisableQueue.Clear();
        }

        private void ClearCallbacks()
        {
            ResetTypeInfosCallback = null;
            OverrideCreateInstanceCallback = null;
            FixedUpdateCallback = null;
            UpdateCallback = null;
            LateUpdateCallback = null;
#if UNITY_EDITOR
            OnDrawGizmosCallback = null;
#endif
        }

        private void FirstAccess()
        {
            if (!m_IsFirstAccessing) return;
            m_IsFirstAccessing = false;
            AutoCreateInstances();
        }

        private void AutoCreateInstances()
        {
            TypeInfo[] autoOrder = m_TypeInfos.Select(kv => kv.Value)
                   .Where(i => i.isAutoInstantiation)
                   .OrderBy(i => i.order)
                   .ToArray();
            for (int i = 0; i < autoOrder.Length; i++)
            { InternalCreateInstance(autoOrder[i].type); }
        }

        private T InternalCreateInstance<T>() where T : GlobalBehaviour
        {
            return InternalCreateInstance(typeof(T)) as T;
        }

        private GlobalBehaviour InternalCreateInstance(in Type type)
        {
            if (!GetTypeInfo(type, out TypeInfo typeInfo)) return null;
            GlobalBehaviour behaviour;
            try
            {
                behaviour = CreateInstanceByTypeInfo(typeInfo);
                if (behaviour == null) return null;
                m_Collection.Add(behaviour);
                behaviour.InernalAwake();
                CheckLifeCycleState(behaviour, StateToCheck.All);
            }
            catch (Exception e)
            {
                if (Debug.isDebugBuild)
                {
                    Debug.LogException(e);
                }
                return null;
            }
            return behaviour;
        }

        private GlobalBehaviour CreateInstanceByTypeInfo(in TypeInfo typeInfo)
        {
            if (OverrideCreateInstanceCallback != null)
            {
                return OverrideCreateInstanceCallback(typeInfo);
            }
            GlobalBehaviour behaviour = Activator.CreateInstance(typeInfo.type) as GlobalBehaviour;
            behaviour.IsExecuteInEditorMode = typeInfo.isExecuteInEditorMode;
            return behaviour;
        }

        private T InternalGetInstance<T>() where T : GlobalBehaviour
        {
            return InternalGetInstance(typeof(T)) as T;
        }

        private GlobalBehaviour InternalGetInstance(in Type type)
        {
            if (!GetTypeInfo(type, out TypeInfo _)) return null;
            return m_Collection.Get(type);
        }

        private T[] InternalGetInstances<T>() where T : GlobalBehaviour
        {
            return InternalGetInstances(typeof(T)).Cast<T>().ToArray();
        }

        private GlobalBehaviour[] InternalGetInstances(in Type type)
        {
            if (!GetTypeInfo(type, out TypeInfo _)) return null;
            return m_Collection.Gets(type);
        }

        private void InternalDestroyInstance(in GlobalBehaviour behaviour)
        {
            if (!GetTypeInfo(behaviour.GetType(), out TypeInfo _)) return;
            try
            {
                behaviour.InternalDestroy();
                m_Collection.Remove(behaviour);
            }
            catch (Exception e)
            {
                if (Debug.isDebugBuild)
                {
                    Debug.LogException(e);
                }
            }
        }

        private bool GetTypeInfo(in Type type, out TypeInfo typeInfo)
        {
            if (!m_TypeInfos.TryGetValue(type.GetHashCode(), out typeInfo))
            {
                if (Debug.isDebugBuild)
                {
                    Debug.LogError($"Type '{type}' is not inherit from type '{typeof(GlobalBehaviour)}'.");
                }
                return false;
            }
            return true;
        }

        internal void FixedUpdate()
        {
            CallUpdate(BehaviourSettings.UpdateMethod.FixedUpdate, FixedUpdateCallback);
        }

        internal void Update()
        {
            CallUpdate(BehaviourSettings.UpdateMethod.Update, UpdateCallback);
        }

        internal void LateUpdate()
        {
            CallUpdate(BehaviourSettings.UpdateMethod.LateUpdate, LateUpdateCallback);
        }

        private void CallUpdate(in BehaviourSettings.UpdateMethod updateMethod, in Action updateCallback)
        {
            if (m_IsReady && UpdateMethod == updateMethod)
            { UpdateLifeCycle(); }
            LogTryCatchEvent(updateCallback);
        }

#if UNITY_EDITOR
        internal void OnDrawGizmos()
        {
            LogTryCatchEvent(OnDrawGizmosCallback);
        }
#endif

        private void LogTryCatchEvent(in Action action)
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception e)
            {
                if (Debug.isDebugBuild)
                {
                    Debug.LogException(e);
                }
            }
        }

        private void UpdateLifeCycle()
        {
            if (Time.realtimeSinceStartupAsDouble - m_LastTime >= DeltaTime)
            {
                m_LastTime = Time.realtimeSinceStartupAsDouble;
                ClearLifeCycleQueues();
                CheckAllLifeCycleState();
                InternalLifeCycleBody();
            }
        }

        private void CheckAllLifeCycleState()
        {
            foreach (GlobalBehaviour behaviour in m_Collection)
            {
                CheckLifeCycleState(behaviour, StateToCheck.All);
            }
        }

        private void CheckLifeCycleState(in GlobalBehaviour behaviour,
            in StateToCheck stateToCheck)
        {
            bool isActived;
            try
            {
                isActived = behaviour.IsActived;
            }
            catch (Exception e)
            {
                if (Debug.isDebugBuild)
                {
                    Debug.LogException(e);
                }
                return;
            }
            bool isLastActived = behaviour.IsLastActived;
            if ((stateToCheck & StateToCheck.Enable) != 0 &&
                !isLastActived && isActived)
            {
                m_EnableQueue.Add(behaviour.ID);
            }
            if ((stateToCheck & StateToCheck.Update) != 0 &&
                isActived)
            {
                m_UpdateQueue.Add(behaviour.ID);
            }
            if ((stateToCheck & StateToCheck.Disable) != 0 &&
                isLastActived && !isActived)
            {
                m_DisableQueue.Add(behaviour.ID);
            }
        }

        private void InternalLifeCycleBody()
        {
            InternalEnable();
            InternalUpdate();
            InternalDisable();
        }

        private void InternalEnable()
        {
            int index = 0;
            while (index < m_EnableQueue.Count)
            {
                int id = m_EnableQueue[index];
                GlobalBehaviour behaviour = m_Collection[id];
                if (behaviour != null)
                {
                    try
                    {
                        behaviour.InernalEnable();
                    }
                    catch (Exception e)
                    {
                        if (Debug.isDebugBuild)
                        {
                            Debug.LogException(e);
                        }
                    }
                    CheckLifeCycleState(behaviour, StateToCheck.UpdateAndDisable);
                }
                index++;
            }
        }

        private void InternalUpdate()
        {
            int index = 0;
            while (index < m_UpdateQueue.Count)
            {
                int id = m_UpdateQueue[index];
                GlobalBehaviour behaviour = m_Collection[id];
                if (behaviour != null)
                {
                    try
                    {
                        behaviour.InternalUpdate();
                    }
                    catch (Exception e)
                    {
                        if (Debug.isDebugBuild)
                        {
                            Debug.LogException(e);
                        }
                    }
                    CheckLifeCycleState(behaviour, StateToCheck.Disable);
                }
                index++;
            }
        }

        private void InternalDisable()
        {
            int index = 0;
            while (index < m_DisableQueue.Count)
            {
                int id = m_DisableQueue[index];
                GlobalBehaviour behaviour = m_Collection[id];
                if (behaviour != null)
                {
                    try
                    {
                        behaviour.InternalDisable();
                    }
                    catch (Exception e)
                    {
                        if (Debug.isDebugBuild)
                        {
                            Debug.LogException(e);
                        }
                    }
                }
                index++;
            }
        }
    }
}