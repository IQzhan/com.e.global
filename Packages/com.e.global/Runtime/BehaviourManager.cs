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

        private SortedList<int, TypeInfo> m_TypeInfos;

        private BehaviourCollection m_Collection;

        private static BehaviourManager m_Instance;

        private static readonly object m_Lock = new object();

        private float m_LastTime;

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
            CollectTypeInfos();
            m_Collection = new BehaviourCollection();
            m_EnableQueue = new List<int>();
            m_UpdateQueue = new List<int>();
            m_DisableQueue = new List<int>();
            BehaviourUpdater.CreateInstance();
            IsReady = true;
        }

        private void Destroy()
        {
            if (!IsReady) return;
            IsReady = false;
            DestroyAllInstances();
            BehaviourUpdater.DestroyInstance();
            m_TypeInfos.Clear();
            m_TypeInfos = null;
            m_Collection.Clear();
            m_Collection = null;
            ClearQueues();
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
            {
                CreateInstance(order[i].type);
            }
        }

        public GlobalBehaviour CreateInstance(in Type type)
        {
            if (!m_TypeInfos.TryGetValue(type.GetHashCode(), out TypeInfo typeInfo))
            {
                throw new Exception($"Type '{type}' is not inherit from type '{typeof(GlobalBehaviour)}'.");
            }
            GlobalBehaviour behaviour = Activator.CreateInstance(type) as GlobalBehaviour;
            behaviour.IsExecuteInEditorMode = typeInfo.isExecuteInEditorMode;
            Debug.Log("AA " + (behaviour != null)); return default;
            m_Collection.Add(behaviour);
            behaviour.InernalAwake();
            CheckToEnable(behaviour);
            CheckToUpdate(behaviour);
            CheckToDisable(behaviour);
            return behaviour;
        }

        public GlobalBehaviour GetInstance(in Type type)
        {
            return m_Collection.Get(type);
        }

        public GlobalBehaviour[] GetInstances(in Type type)
        {
            return m_Collection.Gets(type);
        }

        public void DestroyInstance(in GlobalBehaviour behaviour)
        {
            behaviour.InternalDestroy();
            m_Collection.Remove(behaviour);
        }

        private void DestroyAllInstances()
        {

        }

        internal void FixedUpdate()
        {
            if (IsReady && UpdateMethod == BehaviourSettings.UpdateMethod.FixedUpdate)
            {
                MainUpdate();
            }
            FixedUpdateCallback?.Invoke();
        }

        internal void Update()
        {
            if (IsReady && UpdateMethod == BehaviourSettings.UpdateMethod.Update)
            {
                MainUpdate();
            }
            UpdateCallback?.Invoke();
        }

        internal void LateUpdate()
        {
            if (IsReady && UpdateMethod == BehaviourSettings.UpdateMethod.LateUpdate)
            {
                MainUpdate();
            }
            LateUpdateCallback?.Invoke();
        }

        private void MainUpdate()
        {
            if (Time.realtimeSinceStartup - m_LastTime >= DeltaTime)
            {
                m_LastTime = Time.realtimeSinceStartup;
                UpdateBehaviours();
            }
        }

        private void UpdateBehaviours()
        {
            ClearQueues();
            foreach (GlobalBehaviour behaviour in m_Collection)
            {
                CheckToEnable(behaviour);
                CheckToUpdate(behaviour);
                CheckToDisable(behaviour);
            }
            InternalEnable();
            InternalUpdate();
            InternalDisable();
            ClearQueues();
        }

        private void ClearQueues()
        {
            m_EnableQueue.Clear();
            m_UpdateQueue.Clear();
            m_DisableQueue.Clear();
        }

        private void CheckToEnable(in GlobalBehaviour behaviour)
        {
            if (!behaviour.IsLastActived && behaviour.IsActived)
            {
                m_EnableQueue.Add(behaviour.ID);
            }
        }

        private void CheckToUpdate(in GlobalBehaviour behaviour)
        {
            if (behaviour.IsActived)
            {
                m_UpdateQueue.Add(behaviour.ID);
            }
        }

        private void CheckToDisable(in GlobalBehaviour behaviour)
        {
            if (behaviour.IsLastActived && !behaviour.IsActived)
            {
                m_DisableQueue.Add(behaviour.ID);
            }
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
                    CheckToUpdate(behaviour);
                    CheckToDisable(behaviour);
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
                    CheckToDisable(behaviour);
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
                }
                index++;
            }
        }
    }
}