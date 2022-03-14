namespace E
{
    public abstract partial class GlobalBehaviour
    {
        /// <summary>
        /// Life state of GlobalBehaviour
        /// </summary>
        public enum GlobalBehaviourState
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
    }
}