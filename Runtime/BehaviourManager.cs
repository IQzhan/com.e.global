using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace E
{
    public partial class BehaviourManager
    {
        public bool IsReady { get; private set; }

        public BehaviourSettings.UpdateMethod UpdateMethod { get => BehaviourSettings.Instance.updateMethod; }

        public float DeltaTime { get => BehaviourSettings.Instance.deltaTime; }

        public BehaviourUpdater MonoBehaviour { get => BehaviourUpdater.Instance; }

        public event Action FixedUpdateCallback;

        public event Action UpdateCallback;

        public event Action LateUpdateCallback;

#if UNITY_EDITOR
        public event Action OnDrawGizmosCallback;
#endif

        private SortedList<int, TypeInfo> m_TypeInfos;

        private BehaviourCollection m_Collection;

        private static BehaviourManager m_Instance;

        private static readonly object m_Lock = new object();

        private double m_LastTime;

        private List<int> m_EnableQueue;

        private List<int> m_UpdateQueue;

        private List<int> m_DisableQueue;

        private BehaviourManager() { Initialize(); }

        ~BehaviourManager() { Destroy(); }

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        private static void InitializeOnLoadInEditor()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange stateChange)
        {
            switch (stateChange)
            {
                case PlayModeStateChange.EnteredEditMode:
                    Instance.Initialize();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    Instance.Destroy();
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    Instance.Initialize();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    Instance.Destroy();
                    break;
            }
        }
#else
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeOnLoadInRuntime()
        {
            Instance.Initialize();
        }
#endif

        public static BehaviourManager Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    lock (m_Lock)
                    {
                        if (m_Instance == null)
                        {
                            m_Instance = new BehaviourManager();
                        }
                    }
                }
                return m_Instance;
            }
        }

        private void Initialize()
        {
            if (IsReady) return;
            m_Collection = new BehaviourCollection();
            m_EnableQueue = new List<int>();
            m_UpdateQueue = new List<int>();
            m_DisableQueue = new List<int>();
            CollectTypeInfos();
            BehaviourUpdater.CreateInstance();
            IsReady = true;
        }

        private void Destroy()
        {
            if (!IsReady) return;
            IsReady = false;
            BehaviourUpdater.DestroyInstance();
            DestroyAll();
            ClearLifeCycleQueues();
            m_TypeInfos.Clear();
            m_TypeInfos = null;
            m_EnableQueue = null;
            m_UpdateQueue = null;
            m_DisableQueue = null;
        }

        private void CollectTypeInfos()
        {
            var baseType = typeof(GlobalBehaviour);
            m_TypeInfos = new SortedList<int, TypeInfo>
                (AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes()
                .Where(t => baseType.IsAssignableFrom(t) &&
                t.IsClass &&
                !t.IsAbstract &&
                !t.IsGenericType)
                .Select(t => new TypeInfo(t)))
                .ToDictionary(i => i.type.GetHashCode()));
            TypeInfo[] order = m_TypeInfos.Select(kv => kv.Value)
                .Where(i => i.isAutoInstantiation)
                .OrderBy(i => i.order)
                .ToArray();
            for (int i = 0; i < order.Length; i++)
            { CreateInstance(order[i].type); }
        }

        private void DestroyAll()
        {
            foreach (GlobalBehaviour behaviour in m_Collection)
            {
                behaviour.InternalDestroy();
            }
            m_Collection.Clear();
            m_Collection = null;
        }

        public T CreateInstance<T>() where T : GlobalBehaviour
        {
            return CreateInstance(typeof(T)) as T;
        }

        public GlobalBehaviour CreateInstance(in Type type)
        {
            if (!GetTypeInfo(type, out TypeInfo typeInfo)) return null;
            GlobalBehaviour behaviour = Activator.CreateInstance(type) as GlobalBehaviour;
            behaviour.IsExecuteInEditorMode = typeInfo.isExecuteInEditorMode;
            m_Collection.Add(behaviour);
            behaviour.InernalAwake();
            CheckLifeCycleState(behaviour);
            return behaviour;
        }

        public T GetInstance<T>() where T : GlobalBehaviour
        {
            return GetInstance(typeof(T)) as T;
        }

        public GlobalBehaviour GetInstance(in Type type)
        {
            if (!GetTypeInfo(type, out TypeInfo _)) return null;
            return m_Collection.Get(type);
        }

        public T[] GetInstances<T>() where T : GlobalBehaviour
        {
            return GetInstances(typeof(T)).Cast<T>().ToArray();
        }

        public GlobalBehaviour[] GetInstances(in Type type)
        {
            if (!GetTypeInfo(type, out TypeInfo _)) return null;
            return m_Collection.Gets(type);
        }

        public void DestroyInstance<T>(in T behaviour) where T : GlobalBehaviour
        {
            DestroyInstance(behaviour);
        }

        public void DestroyInstance(in GlobalBehaviour behaviour)
        {
            behaviour.InternalDestroy();
            m_Collection.Remove(behaviour);
        }

        private bool GetTypeInfo(in Type type, out TypeInfo typeInfo)
        {
            if (!m_TypeInfos.TryGetValue(type.GetHashCode(), out typeInfo))
            {
                throw new Exception($"Type '{type}' is not inherit from type '{typeof(GlobalBehaviour)}'.");
            }
            return true;
        }

        internal void FixedUpdate()
        {
            if (IsReady && UpdateMethod == BehaviourSettings.UpdateMethod.FixedUpdate)
            { UpdateLifeCycle(); }
            LogTryCatchEvent(FixedUpdateCallback);
        }

        internal void Update()
        {
            if (IsReady && UpdateMethod == BehaviourSettings.UpdateMethod.Update)
            { UpdateLifeCycle(); }
            LogTryCatchEvent(UpdateCallback);
        }

        internal void LateUpdate()
        {
            if (IsReady && UpdateMethod == BehaviourSettings.UpdateMethod.LateUpdate)
            { UpdateLifeCycle(); }
            LogTryCatchEvent(LateUpdateCallback);
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

        private void ClearLifeCycleQueues()
        {
            m_EnableQueue.Clear();
            m_UpdateQueue.Clear();
            m_DisableQueue.Clear();
        }

        private void CheckAllLifeCycleState()
        {
            foreach (GlobalBehaviour behaviour in m_Collection)
            { CheckLifeCycleState(behaviour); }
        }

        private void CheckLifeCycleState(in GlobalBehaviour behaviour)
        {
            bool isActived = behaviour.IsActived;
            bool isLastActived = behaviour.IsLastActived;
            if (!isLastActived && isActived)
            {
                m_EnableQueue.Add(behaviour.ID);
            }
            if (isActived)
            {
                m_UpdateQueue.Add(behaviour.ID);
            }
            if (isLastActived && !isActived)
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
                    behaviour.InernalEnable();
                    CheckLifeCycleState(behaviour);
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
                    behaviour.InternalUpdate();
                    CheckLifeCycleState(behaviour);
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
                    behaviour.InternalDisable();
                    CheckLifeCycleState(behaviour);
                }
                index++;
            }
        }
    }
}