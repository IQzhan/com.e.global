namespace E
{
    /// <summary>
    /// Simulate <see cref="UnityEngine.MonoBehaviour"/>'s life circle,
    /// all <see cref="GlobalBehaviour"/>s are in one single <see cref="BehaviourUpdater"/>.
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
        /// Life state of GlobalBehaviour
        /// </summary>
        public enum State
        {
            /// <summary>
            /// None -> Awaked
            /// </summary>
            None = 0,

            /// <summary>
            /// Invoking OnAwake
            /// </summary>
            OnAwake = 1,

            /// <summary>
            /// Awaked -> Enabled or Destroyed
            /// </summary>
            Awaked = 2,

            /// <summary>
            /// Invoking OnEnable
            /// </summary>
            OnEnable = 3,

            /// <summary>
            /// Enabled -> Updated or Disabled or (Disabled and Destroyed)
            /// </summary>
            Enabled = 4,

            /// <summary>
            /// Invoking OnUpdate
            /// </summary>
            OnUpdate = 5,

            /// <summary>
            /// Updated -> Disabled or (Disabled and Destroyed)
            /// </summary>
            Updated = 6,

            /// <summary>
            /// Invoking OnDisable
            /// </summary>
            OnDisable = 7,

            /// <summary>
            /// Disabled -> Enabled or Destroyed
            /// </summary>
            Disabled = 8,

            /// <summary>
            /// Invoking OnDestroy
            /// </summary>
            OnDestroy = -1,

            /// <summary>
            /// Destroyed
            /// </summary>
            Destroyed = -2
        }

        /// <summary>
        /// Only runtime id, do not save it.
        /// </summary>
        public int ID => id;

        /// <summary>
        /// Life state of GlobalBehaviour
        /// </summary>
        public State LifeState => m_State;

        /// <summary>
        /// See <see cref="TypeInfo.isExecuteInEditorMode"/>
        /// </summary>
        public bool IsExecuteInEditorMode => isExecuteInEditorMode;

        internal int id = -1;

        internal int typeHashCode;

        internal bool isExecuteInEditorMode = false;

        private State m_State = State.None;

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

        internal void InternalAwake()
        {
            if (!(Utility.IsPlaying || IsExecuteInEditorMode)) return;
            if (m_State == State.None)
            {
                m_State = State.OnAwake;
                try
                {
                    OnAwake();
                    if (m_State == State.OnAwake)
                        m_State = State.Awaked;
                }
                catch (System.Exception e)
                {
                    m_State = State.None;
                    throw e;
                }
            }
        }

        internal void InternalEnable()
        {
            switch (m_State)
            {
                case State.Awaked:
                case State.Disabled:
                    State temp = m_State;
                    try
                    {
                        if (IsActive)
                        {
                            m_State = State.OnEnable;
                            OnEnable();
                            if (m_State == State.OnEnable)
                                m_State = State.Enabled;
                        }
                    }
                    catch (System.Exception e)
                    {
                        m_State = temp;
                        throw e;
                    }
                    break;
            }
        }

        internal void InternalUpdate()
        {
            switch (m_State)
            {
                case State.Enabled:
                case State.Updated:
                    State temp = m_State;
                    try
                    {
                        if (IsActive)
                        {
                            m_State = State.OnUpdate;
                            OnUpdate();
                            if (m_State == State.OnUpdate)
                                m_State = State.Updated;
                        }
                    }
                    catch (System.Exception e)
                    {
                        m_State = temp;
                        throw e;
                    }
                    break;
            }
        }

        internal void InternalDisable()
        {
            switch (m_State)
            {
                case State.Enabled:
                case State.Updated:
                    State temp = m_State;
                    try
                    {
                        if (!IsActive)
                        {
                            m_State = State.OnDisable;
                            OnDisable();
                            if (m_State == State.OnDisable)
                                m_State = State.Disabled;
                        }
                    }
                    catch (System.Exception e)
                    {
                        m_State = temp;
                        throw e;
                    }
                    break;
            }
        }

        internal void InternalDestroy()
        {
            if (m_State < State.OnAwake) return;
            State temp = m_State;
            if (m_State < State.OnDisable)
            {
                try
                {
                    m_State = State.OnDisable;
                    OnDisable();
                }
                catch (System.Exception e)
                {
                    m_State = temp;
                    throw e;
                }
            }
            try
            {
                m_State = State.OnDestroy;
                OnDestroy();
                m_State = State.Destroyed;
            }
            catch (System.Exception e)
            {
                m_State = temp;
                throw e;
            }
        }

        internal bool CheckEnable()
        {
            switch (m_State)
            {
                case State.Awaked:
                case State.Disabled:
                    return IsActive;
            }
            return false;
        }

        internal bool CheckUpdate()
        {
            switch (m_State)
            {
                case State.Enabled:
                case State.Updated:
                    return IsActive;
            }
            return false;
        }

        internal bool CheckDisable()
        {
            switch (m_State)
            {
                case State.Enabled:
                case State.Updated:
                    return !IsActive;
            }
            return false;
        }
    }
}