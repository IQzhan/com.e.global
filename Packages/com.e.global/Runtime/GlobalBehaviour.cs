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
        /// Only runtime id, do not save it.
        /// </summary>
        public int ID { get; internal set; } = -1;

        internal int typeHashCode;

        /// <summary>
        /// See <see cref="TypeInfo.isExecuteInEditorMode"/>
        /// </summary>
        public bool IsExecuteInEditorMode { get; internal set; } = false;

        /// <summary>
        /// Can execute life circle methods?
        /// </summary>
        public bool IsAlive { get => Utility.IsPlaying || IsExecuteInEditorMode; }

        /// <summary>
        /// <see cref="OnAwake()"/> has already callated.
        /// </summary>
        public bool IsAwaked { get; private set; } = false;

        internal bool IsLastActived { get; private set; } = false;

        /// <summary>
        /// Allow to call <see cref="OnUpdate()"/>
        /// </summary>
        public bool IsActived { get => IsAwaked && IsEnabled; }

        /// <summary>
        /// Control enable state.
        /// </summary>
        protected abstract bool IsEnabled { get; }

        /// <summary>
        /// Call by <see cref="BehaviourManager.CreateInstance"/> if <see cref="IsAlive"/> is true.
        /// </summary>
        protected virtual void OnAwake() { }

        /// <summary>
        /// Call if <see cref="IsActived"/> is from false to true.
        /// </summary>
        protected virtual void OnEnable() { }

        /// <summary>
        /// Call if <see cref="IsActived"/> is true.
        /// </summary>
        protected virtual void OnUpdate() { }

        /// <summary>
        /// Call if <see cref="IsActived"/> is from true to false.
        /// </summary>
        protected virtual void OnDisable() { }

        /// <summary>
        /// Call by <see cref="BehaviourManager.DestroyInstance"/> if <see cref="IsAwaked"/> is true.
        /// </summary>
        protected virtual void OnDestroy() { }

        internal void InernalAwake()
        {
            if (IsAlive && !IsAwaked && !IsLastActived)
            {
                OnAwake();
                IsAwaked = true;
            }
        }

        internal void InernalEnable()
        {
            if (!IsLastActived && IsActived)
            {
                OnEnable();
                IsLastActived = true;
            }
        }

        internal void InternalUpdate()
        {
            if (IsActived)
            {
                OnUpdate();
                IsLastActived = true;
            }
        }

        internal void InternalDisable()
        {
            if (IsLastActived && !IsActived)
            {
                OnDisable();
                IsLastActived = false;
            }
        }

        internal void InternalDestroy()
        {
            if (IsAwaked)
            {
                if (IsLastActived)
                {
                    OnDisable();
                    IsLastActived = false;
                }
                OnDestroy();
                IsAwaked = false;
            }
        }
    }
}