using System;
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
        #region Public properties

        public static bool IsReady { get => m_Instance.m_IsReady; }

        public static BehaviourSettings.UpdateMethod UpdateMethod { get => BehaviourSettings.Method; }

        public static float DeltaTime { get => BehaviourSettings.DeltaTime; }

        public static BehaviourUpdater MonoBehaviour { get => BehaviourUpdater.Instance; }

        public static event Func<IEnumerable<TypeInfo>, IEnumerable<TypeInfo>> ResetTypeInfosCallback;

        public static event Func<TypeInfo, GlobalBehaviour> OverrideCreateInstanceCallback;

        public static event Action FixedUpdateCallback;

        public static event Action UpdateCallback;

        public static event Action LateUpdateCallback;

#if UNITY_EDITOR
        public static event Action OnDrawGizmosCallback;
#endif
        #endregion

        #region Private properties

        private static BehaviourManager m_Instance;

        private bool m_IsReady;

        private SortedList<int, TypeInfo> m_TypeInfos;

        private BehaviourCollection m_Collection;

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

        #endregion

        #region Public methods

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

        #endregion

        #region Initialize & Dispose

        static BehaviourManager()
        { m_Instance = new BehaviourManager(); }

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
                case PlayModeStateChange.EnteredPlayMode:
                    InitializeOnLoadInRuntime();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                case PlayModeStateChange.ExitingPlayMode:
                    break;
            }
        }

        [UnityEditor.Callbacks.DidReloadScripts]
#else
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif
        private static void InitializeOnLoadInRuntime()
        {
            m_Instance.FirstAccess();
            BehaviourUpdater.Instance.manager = m_Instance;
        }

        private void Initialize()
        {
            try
            {
                ClearCallbacks();
                CreateLifeCycleQueues();
            }
            catch (Exception e)
            {
                if (Utility.AllowLogError)
                {
                    Utility.LogException(e);
                }
                return;
            }
        }

        private void Destroy()
        {
            try
            {
                BehaviourUpdater.DestroyInstance();
                ReleaseCollection();
                ReleaseLifeCycleQueues();
                ClearCallbacks();
                ReleaseTypeInfos();
                GC.SuppressFinalize(this);
            }
            catch (Exception e)
            {
                if (Utility.AllowLogError)
                {
                    Utility.LogException(e);
                }
                return;
            }
        }

        private void FirstAccess()
        {
            try
            {
                if (m_IsReady) return;
                IEnumerable<Type> types = GetAllTypes();
                ExecuteBefore(types);
                CreateTypeInfos(types);
                AutoCreateInstances();
                m_IsReady = true;
            }
            catch (Exception e)
            {
                if (Utility.AllowLogError)
                {
                    Utility.LogException(e);
                }
            }
        }

        private IEnumerable<Type> GetAllTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes());
        }

        private void ExecuteBefore(IEnumerable<Type> allTypes)
        {
            Type attrType = typeof(InitializeBeforeAllBehavioursMethodAttribute);
            var methods = allTypes
                .SelectMany(t => t.GetMethods()
                .Where(m => m.IsStatic && !m.IsGenericMethod && !m.IsConstructor &&
                (m.GetCustomAttributes(attrType, true).FirstOrDefault() is InitializeBeforeAllBehavioursMethodAttribute) &&
                (m.GetParameters().Length == 0)));
            var paramObjs = new object[0];
            foreach (var method in methods)
            {
                try
                {
                    method.Invoke(null, paramObjs);
                }
                catch (Exception e)
                {
                    if (Utility.AllowLogError)
                    {
                        Utility.LogException(e);
                    }
                }
            }
        }

        private void CreateTypeInfos(IEnumerable<Type> allTypes)
        {
            var baseType = typeof(GlobalBehaviour);
            IEnumerable<TypeInfo> typeInfos = allTypes
                .Where(t => baseType.IsAssignableFrom(t) &&
                t.IsClass &&
                !t.IsAbstract &&
                !t.IsGenericType)
                .Select(t => new TypeInfo(t))
                // ToList() makes sure this linq excuted.
                .ToList();
            if (ResetTypeInfosCallback != null)
            { typeInfos = ResetTypeInfosCallback(typeInfos); }
            m_TypeInfos = new SortedList<int, TypeInfo>
                (typeInfos.ToDictionary(i => i.TypeHashCode));
            m_Collection = new BehaviourCollection(typeInfos);
        }

        private void AutoCreateInstances()
        {
            TypeInfo[] autoOrder = m_TypeInfos.Select(kv => kv.Value)
                   .Where(i => i.isAutoInstantiation)
                   .OrderBy(i => i.order)
                   .ToArray();
            for (int i = 0; i < autoOrder.Length; i++)
            { InternalCreateInstance(autoOrder[i].Value); }
        }

        private void ReleaseTypeInfos()
        {
            if (m_TypeInfos == null) return;
            m_TypeInfos.Clear();
            m_TypeInfos = null;
        }

        private void ReleaseCollection()
        {
            if (m_Collection == null) return;
            foreach (GlobalBehaviour behaviour in m_Collection)
            {
                try
                {
                    behaviour.InternalDestroy();
                }
                catch (Exception e)
                {
                    if (Utility.AllowLogError)
                    {
                        Utility.LogException(e);
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
            ReleaseLifeCycleQueue(ref m_EnableQueue);
            ReleaseLifeCycleQueue(ref m_UpdateQueue);
            ReleaseLifeCycleQueue(ref m_DisableQueue);
        }

        private void ReleaseLifeCycleQueue(ref List<int> queue)
        {
            if (queue != null)
            {
                queue.Clear();
                queue = null;
            }
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

        #endregion

        #region Life Cycle methods

        private T InternalCreateInstance<T>() where T : GlobalBehaviour
        {
            return InternalCreateInstance(typeof(T)) as T;
        }

        private GlobalBehaviour InternalCreateInstance(in Type type)
        {
            int typeHashCode = type.GetHashCode();
            if (!GetTypeInfo(typeHashCode, out TypeInfo typeInfo)) return null;
            GlobalBehaviour behaviour;
            try
            {
                behaviour = CreateInstanceByTypeInfo(typeInfo);
                if (behaviour == null)
                {
                    if (Utility.AllowLogError)
                    {
                        Utility.LogError($"Create GlobalBehaviour of type '{type}' faild.");
                    }
                    return null;
                }
                m_Collection.Add(behaviour);
                behaviour.InernalAwake();
                CheckLifeCycleState(behaviour, StateToCheck.All);
            }
            catch (Exception e)
            {
                if (Utility.AllowLogError)
                {
                    Utility.LogException(e);
                }
                return null;
            }
            return behaviour;
        }

        private GlobalBehaviour CreateInstanceByTypeInfo(in TypeInfo typeInfo)
        {
            GlobalBehaviour behaviour;
            if (OverrideCreateInstanceCallback != null)
            {
                behaviour = OverrideCreateInstanceCallback(typeInfo);
            }
            else
            {
                behaviour = Activator.CreateInstance(typeInfo.Value, true) as GlobalBehaviour;
            }
            behaviour.typeHashCode = typeInfo.TypeHashCode;
            behaviour.IsExecuteInEditorMode = typeInfo.isExecuteInEditorMode;
            return behaviour;
        }

        private T InternalGetInstance<T>() where T : GlobalBehaviour
        {
            return InternalGetInstance(typeof(T)) as T;
        }

        private GlobalBehaviour InternalGetInstance(in Type type)
        {
            int typeHashCode = type.GetHashCode();
            if (!GetTypeInfo(typeHashCode, out TypeInfo _)) return null;
            return m_Collection.Get(typeHashCode);
        }

        private T[] InternalGetInstances<T>() where T : GlobalBehaviour
        {
            int typeHashCode = typeof(T).GetHashCode();
            if (!GetTypeInfo(typeHashCode, out TypeInfo _)) return null;
            return m_Collection.Gets<T>(typeHashCode);
        }

        private GlobalBehaviour[] InternalGetInstances(in Type type)
        {
            int typeHashCode = type.GetHashCode();
            if (!GetTypeInfo(typeHashCode, out TypeInfo _)) return null;
            return m_Collection.Gets(typeHashCode);
        }

        private void InternalDestroyInstance(in GlobalBehaviour behaviour)
        {
            if (!GetTypeInfo(behaviour.typeHashCode, out TypeInfo _)) return;
            try
            {
                behaviour.InternalDestroy();
                m_Collection.Remove(behaviour);
            }
            catch (Exception e)
            {
                if (Utility.AllowLogError)
                {
                    Utility.LogException(e);
                }
            }
        }

        private bool GetTypeInfo(int typeHashCode, out TypeInfo typeInfo)
        {
            if (!m_TypeInfos.TryGetValue(typeHashCode, out typeInfo))
            {
                if (Utility.AllowLogError)
                {
                    Utility.LogError($"Type '{typeInfo.Value}' is not inherit from type '{typeof(GlobalBehaviour)}'.");
                }
                return false;
            }
            return true;
        }

        internal void FixedUpdate()
        { CallUpdate(BehaviourSettings.UpdateMethod.FixedUpdate, FixedUpdateCallback); }

        internal void Update()
        { CallUpdate(BehaviourSettings.UpdateMethod.Update, UpdateCallback); }

        internal void LateUpdate()
        { CallUpdate(BehaviourSettings.UpdateMethod.LateUpdate, LateUpdateCallback); }

        private void CallUpdate(BehaviourSettings.UpdateMethod updateMethod, in Action updateCallback)
        {
            if (m_IsReady)
            {
                if (UpdateMethod == updateMethod) UpdateLifeCycle();
                LogTryCatchEvent(updateCallback);
            }
        }

#if UNITY_EDITOR
        internal void OnDrawGizmos()
        { LogTryCatchEvent(OnDrawGizmosCallback); }
#endif

        private void LogTryCatchEvent(in Action action)
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception e)
            {
                if (Utility.AllowLogError)
                {
                    Utility.LogException(e);
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

        private void CheckLifeCycleState(in GlobalBehaviour behaviour, StateToCheck stateToCheck)
        {
            bool isActived;
            try
            {
                isActived = behaviour.IsActived;
            }
            catch (Exception e)
            {
                if (Utility.AllowLogError)
                {
                    Utility.LogException(e);
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
                        if (Utility.AllowLogError)
                        {
                            Utility.LogException(e);
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
                        if (Utility.AllowLogError)
                        {
                            Utility.LogException(e);
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
                        if (Utility.AllowLogError)
                        {
                            Utility.LogException(e);
                        }
                    }
                }
                index++;
            }
        }

        #endregion
    }
}