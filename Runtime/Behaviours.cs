using System.Collections.Generic;
using UnityEngine;

namespace E
{
    [Singleton(Name = "Behaviours", Persistent = true)]
    [AddComponentMenu("Global/Behaviours")]
    [ExecuteAlways]
    public sealed partial class Behaviours : Singleton<Behaviours>
    {
        private bool m_QueueInitialized;

        private List<int> m_AwakeQueue;

        private List<int> m_EnableQueue;

        private List<int> m_ActiveQueue;

        private List<int> m_DisableQueue;

        private List<GlobalBehaviour> m_DestroyQueue;

        private int m_SelectedIndex;

        [SerializeField, HideInInspector]
        private GlobalBehaviourOrders m_Settings;

        private List<GlobalBehaviour> Orders
        { get { return m_Settings != null ? m_Settings.Orders : null; } }

        internal GlobalBehaviourOrders Settings
        {
            get
            {
                return m_Settings;
            }
            set
            {
                GlobalBehaviourOrders temp = m_Settings;
                m_Settings = value;
                if (temp != null && value != temp)
                {
                    temp.CheckAllToDestroyQueue();
                }
            }
        }

#if UNITY_EDITOR
        [UnityEditor.Callbacks.DidReloadScripts]
#endif
        private static void RecreateInstance()
        {
            if (Exists)
            {
                Behaviours inst = Instance;
                inst.Destroy();
                inst.Create();
            }
        }

        internal bool IsPlaying(in GlobalBehaviourOrders settings)
        {
            return settings == m_Settings;
        }

        internal List<GlobalBehaviour> GetDestroyQueue()
        {
            return m_DestroyQueue;
        }

        protected override void Create()
        {
            m_SelectedIndex = -1;
            CreateQueues();
        }

        private void Update()
        {
            if (!OrdersReady(out List<GlobalBehaviour> orders)) return;
            CheckOrders(orders);
            ExecuteOrders(orders);
            ExecuteDestroy();
        }

        protected override void Destroy()
        {
            if (m_Settings != null)
            {
                m_Settings.CheckAllToDestroyQueue();
            }
            ExecuteDestroy();
            ReleaseQueues();
            m_SelectedIndex = -1;
        }

        private bool OrdersReady(out List<GlobalBehaviour> orders)
        {
            orders = Orders;
            return m_QueueInitialized && (orders != null);
        }

        private void CreateQueues()
        {
            m_AwakeQueue = new List<int>();
            m_ActiveQueue = new List<int>();
            m_EnableQueue = new List<int>();
            m_DisableQueue = new List<int>();
            m_DestroyQueue = new List<GlobalBehaviour>();
            m_QueueInitialized = true;
        }

        private void ReleaseQueues()
        {
            ReleaseList(ref m_AwakeQueue);
            ReleaseList(ref m_EnableQueue);
            ReleaseList(ref m_ActiveQueue);
            ReleaseList(ref m_DisableQueue);
            ReleaseList(ref m_DestroyQueue);
            m_QueueInitialized = false;
        }

        private void ReleaseList<T>(ref List<T> list)
        {
            if (list != null)
            {
                list.Clear();
                list = null;
            }
        }

        private void CheckOrders(in List<GlobalBehaviour> orders)
        {
            m_AwakeQueue.Clear();
            m_EnableQueue.Clear();
            m_ActiveQueue.Clear();
            m_DisableQueue.Clear();
            for (int i = 0; i < orders.Count; i++)
            {
                GlobalBehaviour behaviour = orders[i];
                if (behaviour == null) continue;
                behaviour.Check(
                    out bool awakeState,
                    out bool enableState,
                    out bool activeState,
                    out bool disableState);
                if (awakeState) m_AwakeQueue.Add(i);
                if (enableState) m_EnableQueue.Add(i);
                if (activeState) m_ActiveQueue.Add(i);
                if (disableState) m_DisableQueue.Add(i);
            }
        }

        private void ExecuteOrders(in List<GlobalBehaviour> orders)
        {
            ExecuteAwake(orders);
            ExecuteEnable(orders);
            ExecuteUpdate(orders);
            ExecuteDisable(orders);
        }

        private void ExecuteAwake(in List<GlobalBehaviour> orders)
        {
            for (int i = 0; i < orders.Count; i++)
            {
                int index = m_AwakeQueue[i];
                GlobalBehaviour behaviour = orders[index];
                behaviour.ExecuteAwake();
            }
            m_AwakeQueue.Clear();
        }

        private void ExecuteEnable(in List<GlobalBehaviour> orders)
        {
            for (int i = 0; i < m_EnableQueue.Count; i++)
            {
                int index = m_EnableQueue[i];
                GlobalBehaviour behaviour = orders[index];
                behaviour.ExecuteEnable();
            }
            m_EnableQueue.Clear();
        }

        private void ExecuteUpdate(in List<GlobalBehaviour> orders)
        {
            for (int i = 0; i < m_ActiveQueue.Count; i++)
            {
                int index = m_ActiveQueue[i];
                GlobalBehaviour behaviour = orders[index];
                behaviour.ExecuteUpdate();
            }
            m_ActiveQueue.Clear();
        }

        private void ExecuteDisable(in List<GlobalBehaviour> orders)
        {
            for (int i = 0; i < m_DisableQueue.Count; i++)
            {
                int index = m_DisableQueue[i];
                GlobalBehaviour behaviour = orders[index];
                behaviour.ExecuteDisable();
            }
            m_DisableQueue.Clear();
        }

        private void ExecuteDestroy()
        {
            for (int i = 0; i < m_DestroyQueue.Count; i++)
            {
                GlobalBehaviour behaviour = m_DestroyQueue[i];
                behaviour.ExecuteDestroy();
            }
            m_DestroyQueue.Clear();
        }
    }
}