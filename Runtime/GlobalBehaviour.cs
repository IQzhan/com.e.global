namespace E
{
    /// <summary>
    /// Simulate <see cref="UnityEngine.MonoBehaviour"/>'s life circle,
    /// all <see cref="GlobalBehaviour"/>s are in one single <see cref="GlobalUpdater"/>.
    /// <para>See also:
    /// <seealso cref="BehaviourManager"/>,
    /// <seealso cref="TypeInfo"/>,
    /// <seealso cref="AutoInstantiateAttribute"/>,
    /// <seealso cref="UnityEngine.ExecuteInEditMode"/>,
    /// <seealso cref="UnityEngine.ExecuteAlways"/></para>
    /// </summary>
    public abstract partial class GlobalBehaviour
    {
        /// <summary>
        /// Only runtime id, do not save it.
        /// </summary>
        public int ID => id;

        /// <summary>
        /// Life state of GlobalBehaviour
        /// </summary>
        public GlobalBehaviourState LifeState => m_State;

        /// <summary>
        /// See <see cref="TypeInfo.isExecuteInEditorMode"/>
        /// </summary>
        public bool IsExecuteInEditorMode => isExecuteInEditorMode;

        internal int id = -1;

        internal int typeHashCode;

        internal bool isExecuteInEditorMode = false;

        private GlobalBehaviourState m_State = GlobalBehaviourState.None;

        /// <summary>
        /// Control enable state.
        /// </summary>
        protected abstract bool IsActive { get; }

        /// <summary>
        /// Call by <see cref="BehaviourManager.CreateInstance"/>.
        /// </summary>
        protected virtual void OnAwake() { }

        /// <summary>
        /// Call if <see cref="IsActive"/> is from false to true.
        /// </summary>
        protected virtual void OnEnable() { }

        /// <summary>
        /// Call if <see cref="IsActive"/> is true.
        /// </summary>
        protected virtual void OnUpdate() { }

        /// <summary>
        /// Call if <see cref="IsActive"/> is from true to false.
        /// </summary>
        protected virtual void OnDisable() { }

        /// <summary>
        /// Call by <see cref="BehaviourManager.DestroyInstance"/>.
        /// </summary>
        protected virtual void OnDestroy() { }

        internal bool InternalAwake()
        {
            if (!(Utility.IsPlaying || IsExecuteInEditorMode)) return false;
            if (m_State == GlobalBehaviourState.None)
            {
                m_State = GlobalBehaviourState.OnAwake;
                try
                {
                    OnAwake();
                    if (m_State == GlobalBehaviourState.OnAwake)
                        m_State = GlobalBehaviourState.Awaked;
                    return true;
                }
                catch (System.Exception e)
                {
                    m_State = GlobalBehaviourState.None;
                    if (Utility.AllowLogError)
                    {
                        Utility.LogException(e);
                    }
                }
            }
            return false;
        }

        internal bool InternalEnable()
        {
            switch (m_State)
            {
                case GlobalBehaviourState.Awaked:
                case GlobalBehaviourState.Disabled:
                    GlobalBehaviourState temp = m_State;
                    try
                    {
                        if (IsActive)
                        {
                            m_State = GlobalBehaviourState.OnEnable;
                            OnEnable();
                            if (m_State == GlobalBehaviourState.OnEnable)
                                m_State = GlobalBehaviourState.Enabled;
                            return true;
                        }
                    }
                    catch (System.Exception e)
                    {
                        m_State = temp;
                        if (Utility.AllowLogError)
                        {
                            Utility.LogException(e);
                        }
                    }
                    break;
            }
            return false;
        }

        internal bool InternalUpdate()
        {
            switch (m_State)
            {
                case GlobalBehaviourState.Enabled:
                case GlobalBehaviourState.Updated:
                    GlobalBehaviourState temp = m_State;
                    try
                    {
                        if (IsActive)
                        {
                            m_State = GlobalBehaviourState.OnUpdate;
                            OnUpdate();
                            if (m_State == GlobalBehaviourState.OnUpdate)
                                m_State = GlobalBehaviourState.Updated;
                            return true;
                        }
                    }
                    catch (System.Exception e)
                    {
                        m_State = temp;
                        if (Utility.AllowLogError)
                        {
                            Utility.LogException(e);
                        }
                    }
                    break;
            }
            return false;
        }

        internal bool InternalDisable()
        {
            switch (m_State)
            {
                case GlobalBehaviourState.Enabled:
                case GlobalBehaviourState.Updated:
                    GlobalBehaviourState temp = m_State;
                    try
                    {
                        if (!IsActive)
                        {
                            m_State = GlobalBehaviourState.OnDisable;
                            OnDisable();
                            if (m_State == GlobalBehaviourState.OnDisable)
                                m_State = GlobalBehaviourState.Disabled;
                            return true;
                        }
                    }
                    catch (System.Exception e)
                    {
                        m_State = temp;
                        if (Utility.AllowLogError)
                        {
                            Utility.LogException(e);
                        }
                    }
                    break;
            }
            return false;
        }

        internal bool InternalDestroy()
        {
            if (m_State < GlobalBehaviourState.OnAwake) return false;
            GlobalBehaviourState temp = m_State;
            if (m_State < GlobalBehaviourState.OnDisable)
            {
                try
                {
                    m_State = GlobalBehaviourState.OnDisable;
                    OnDisable();
                }
                catch (System.Exception e)
                {
                    m_State = temp;
                    if (Utility.AllowLogError)
                    {
                        Utility.LogException(e);
                    }
                    return false;
                }
            }
            try
            {
                m_State = GlobalBehaviourState.OnDestroy;
                OnDestroy();
                m_State = GlobalBehaviourState.Destroyed;
                return true;
            }
            catch (System.Exception e)
            {
                m_State = temp;
                if (Utility.AllowLogError)
                {
                    Utility.LogException(e);
                }
                return false;
            }
        }

        internal bool CheckEnable()
        {
            switch (m_State)
            {
                case GlobalBehaviourState.Awaked:
                case GlobalBehaviourState.Disabled:
                    return IsActive;
            }
            return false;
        }

        internal bool CheckUpdate()
        {
            switch (m_State)
            {
                case GlobalBehaviourState.Enabled:
                case GlobalBehaviourState.Updated:
                    return IsActive;
            }
            return false;
        }

        internal bool CheckDisable()
        {
            switch (m_State)
            {
                case GlobalBehaviourState.Enabled:
                case GlobalBehaviourState.Updated:
                    return !IsActive;
            }
            return false;
        }
    }
}