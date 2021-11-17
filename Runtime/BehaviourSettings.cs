using UnityEngine;

namespace E
{
    public class BehaviourSettings : ScriptableObject
    {
        public enum UpdateMethod
        {
            Update,
            LateUpdate,
            FixedUpdate
        }

        private static BehaviourSettings m_Instance;

        private static BehaviourSettings Instance
        {
            get
            {
                if (m_Instance == null)
                {
#if UNITY_EDITOR
                    Utility.CreateAssetIfNotExists<BehaviourSettings>();
#endif
                    m_Instance = Utility.Load<BehaviourSettings>();
                }
                return m_Instance;
            }
        }

        public static float DeltaTime { get => Instance.m_DeltaTime; set => Instance.m_DeltaTime = value; }

        public static UpdateMethod Method { get => Instance.m_Method; set => Instance.m_Method = value; }

        public static bool AllowLog { get => Instance.m_AllowLog; set => Instance.m_AllowLog = value; }

        public static bool AllowLogError { get => Instance.m_AllowLogError; set => Instance.m_AllowLogError = value; }

        [SerializeField]
        private float m_DeltaTime = 1f / 130f;

        [SerializeField]
        private UpdateMethod m_Method = UpdateMethod.Update;

        [SerializeField]
        private bool m_AllowLog = false;

        [SerializeField]
        private bool m_AllowLogError = true;
    }
}