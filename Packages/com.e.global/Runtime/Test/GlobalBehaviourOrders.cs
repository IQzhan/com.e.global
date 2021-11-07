using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SubsystemsImplementation;

namespace E
{
    public class GlobalBehaviourOrders : ScriptableObject
    {
        [SerializeField, HideInInspector]
        private List<GlobalBehaviour_0> orders;

        internal List<GlobalBehaviour_0> Orders
        {
            get
            {
                if (orders == null)
                {
                    orders = new List<GlobalBehaviour_0>();
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

        public GlobalBehaviour_0 GetObject(in Type type)
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

        public GlobalBehaviour_0[] GetObjects(in Type type)
        {
            List<GlobalBehaviour_0> result = new List<GlobalBehaviour_0>();
            for (int i = 0; i < orders.Count; i++)
            {
                if (orders[i].GetType().Equals(type))
                {
                    result.Add(orders[i]);
                }
            }
            return result.ToArray();
        }

        public GlobalBehaviour_0[] GetObjects()
        {
            return orders.ToArray();
        }

        public GlobalBehaviour_0 CreateObject(in Type type)
        {
            GlobalBehaviour_0 obj = CreateInstance(type) as GlobalBehaviour_0;
            orders.Add(obj);
            return obj;
        }

        public void RemoveObject(in GlobalBehaviour_0 obj)
        {
            if (obj == null) return;
            orders.Remove(obj);
            TryAddToDestroyQueue(obj);
        }

        private void TryAddToDestroyQueue(in GlobalBehaviour_0 obj)
        {
            if (IsPlaying && obj.Initialized)
            {
                List<GlobalBehaviour_0> destroyQueue = Behaviours.Instance.GetDestroyQueue();
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
            List<GlobalBehaviour_0> destroyQueue = Behaviours.Instance.GetDestroyQueue();
            for (int i = 0; i < orders.Count; i++)
            {
                GlobalBehaviour_0 behaviour = orders[i];
                if (behaviour != null && behaviour.Initialized)
                {
                    destroyQueue.Add(behaviour);
                }
            }
        }
    }
}