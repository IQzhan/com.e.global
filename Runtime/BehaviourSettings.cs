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

        private static BehaviourSettings instance;

        public static BehaviourSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = BehaviourUtility.Load<BehaviourSettings>(BehaviourUtility.SettingsName);
                }
                return instance;
            }
        }

        public float deltaTime;

        public UpdateMethod updateMethod = UpdateMethod.Update;
    }
}