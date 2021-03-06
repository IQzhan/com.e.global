using UnityEngine;

namespace E
{
    /// <summary>
    /// Settings of global system.
    /// </summary>
    public class GlobalSettings : ScriptableObject
    {
        public enum UpdateMethod
        {
            Update,
            LateUpdate,
            FixedUpdate
        }

        private static GlobalSettings m_Instance;

        private static GlobalSettings Instance
        {
            get
            {
                if (m_Instance == null)
                {
#if UNITY_EDITOR
                    Utility.CreateAssetIfNotExists<GlobalSettings>();
#endif
                    m_Instance = Utility.Load<GlobalSettings>();
                }
                return m_Instance;
            }
        }

        /// <summary>
        /// Update delta time.
        /// </summary>
        public static double DeltaTime { get => Instance.m_DeltaTime; set => Instance.m_DeltaTime = value; }

        /// <summary>
        /// Use which method to update.
        /// </summary>
        public static UpdateMethod Method { get => Instance.m_Method; set => Instance.m_Method = value; }

        public static bool AllowLog { get => Instance.m_AllowLog; set => Instance.m_AllowLog = value; }

        public static bool AllowLogError { get => Instance.m_AllowLogError; set => Instance.m_AllowLogError = value; }

        [SerializeField, Min(0.001f)]
        private double m_DeltaTime = 1f / 130f;

        [SerializeField]
        private UpdateMethod m_Method = UpdateMethod.Update;

        [SerializeField]
        private bool m_AllowLog = false;

        [SerializeField]
        private bool m_AllowLogError = true;
    }
}