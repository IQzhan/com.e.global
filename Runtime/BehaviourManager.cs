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

        private ulong m_IDCount;

        private BehaviourCollection m_Collection;

        private float m_LastTime;

        public static void Create()
        {
            Instance.Initialize();
        }

        private void Initialize()
        {
            m_Collection = new BehaviourCollection();
            IsReady = true;
        }

        private ulong CreateID()
        {
            return ++m_IDCount;
        }

        public GlobalBehaviour CreateInstance(in Type type)
        {
            //collection.Add();
            ulong id = CreateID();
            return null;
        }

        public GlobalBehaviour GetInstance(in Type type)
        {
            return m_Collection.Get(type);
        }

        public GlobalBehaviour[] GetInstances(in Type type)
        {
            return m_Collection.Gets(type);
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