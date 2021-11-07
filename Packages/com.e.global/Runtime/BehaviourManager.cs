using System;
using UnityEngine;

namespace E
{
    public partial class BehaviourManager
    {
        private static BehaviourManager instance;

        public static BehaviourManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BehaviourManager();
                }
                return instance;
            }
        }

        public bool IsReady { get; private set; }

        public BehaviourSettings.UpdateMethod UpdateMethod { get => BehaviourSettings.Instance.updateMethod; }

        public float DeltaTime { get => BehaviourSettings.Instance.deltaTime; }

        public BehaviourUpdater MonoBehaviour { get => BehaviourUpdater.Instance; }

        private BehaviourCollection collection;

        private float m_LastTime;

        public static void Create()
        {
            Instance.Initialize();
        }

        private void Initialize()
        {
            collection = new BehaviourCollection();
            IsReady = true;
        }

        public GlobalBehaviour CreateInstance(in Type type)
        {
            //collection.Add();
            return null;
        }

        public GlobalBehaviour GetInstance(in Type type)
        {
            return collection.Get(type);
        }

        public GlobalBehaviour[] GetInstances(in Type type)
        {
            return collection.Gets(type);
        }

        public void DestroyInstance(GlobalBehaviour behaviour)
        {

        }

        public void Update()
        {
            if(Time.realtimeSinceStartup - m_LastTime > DeltaTime)
            {
                m_LastTime = Time.realtimeSinceStartup;

            }
        }


    }
}