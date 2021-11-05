using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SubsystemsImplementation;

namespace E
{
    public class GlobalBehaviourOrders : ScriptableObject
    {
        [SerializeField, HideInInspector]
        private List<GlobalBehaviour> orders;

        internal List<GlobalBehaviour> Orders
        {
            get
            {
                if (orders == null)
                {
                    orders = new List<GlobalBehaviour>();
                }
                return orders;
            }
        }

        public bool IsPlaying
        {
            get
            {
                if (Behaviours.Exists)
                {
                    return Behaviours.Instance.IsPlaying(this);
                }
                return false;
            }
        }

        public int ObjectCount
        {
            get
            {
                return orders.Count;
            }
        }

        public GlobalBehaviour GetObject(in Type type)
        {
            for (int i = 0; i < orders.Count; i++)
            {
                if (orders[i].GetType().Equals(type))
                {
                    return orders[i];
                }
            }
            return null;
        }

        public GlobalBehaviour[] GetObjects(in Type type)
        {
            List<GlobalBehaviour> result = new List<GlobalBehaviour>();
            for (int i = 0; i < orders.Count; i++)
            {
                if (orders[i].GetType().Equals(type))
                {
                    result.Add(orders[i]);
                }
            }
            return result.ToArray();
        }

        public GlobalBehaviour[] GetObjects()
        {
            return orders.ToArray();
        }

        public GlobalBehaviour CreateObject(in Type type)
        {
            GlobalBehaviour obj = CreateInstance(type) as GlobalBehaviour;
            orders.Add(obj);
            return obj;
        }

        public void RemoveObject(in GlobalBehaviour obj)
        {
            if (obj == null) return;
            orders.Remove(obj);
            TryAddToDestroyQueue(obj);
        }

        private void TryAddToDestroyQueue(in GlobalBehaviour obj)
        {
            if (IsPlaying && obj.Initialized)
            {
                List<GlobalBehaviour> destroyQueue = Behaviours.Instance.GetDestroyQueue();
                destroyQueue.Add(obj);
                obj.actualDestroy = true;
            }
            else
            {
                if (Application.isPlaying)
                {
                    Destroy(obj);
                }
                else
                {
                    DestroyImmediate(obj);
                }
            }
        }

        internal void CheckAllToDestroyQueue()
        {
            if (!IsPlaying) return;
            List<GlobalBehaviour> destroyQueue = Behaviours.Instance.GetDestroyQueue();
            for (int i = 0; i < orders.Count; i++)
            {
                GlobalBehaviour behaviour = orders[i];
                if (behaviour != null && behaviour.Initialized)
                {
                    destroyQueue.Add(behaviour);
                }
            }
        }
    }
}