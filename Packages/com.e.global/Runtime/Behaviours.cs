using System.Collections.Generic;
using UnityEngine;

namespace E
{
    [Singleton(Name = "Behaviours", Persistent = true)]
    [AddComponentMenu("")]
    [ExecuteAlways]
    internal partial class Behaviours : Singleton<Behaviours>
    {
        private bool m_QueueInitialized;

        private List<int> m_AwakeQueue;

        private List<int> m_EnableQueue;

        private List<int> m_ActiveQueue;

        private List<int> m_DisableQueue;

        private List<int> m_DestroyQueue;

        private int m_SelectedIndex;

        [SerializeField]
        private GlobalBehaviourOrders m_Settings;

        private GlobalBehaviour[] Orders
        { get { return m_Settings?.Orders; } }

        // ‘⁄±‡“Î∫Û÷¥––
        private static void RecreateInstance()
        {
            if (Exists)
            {
                Behaviours inst = Instance;
                inst.Destroy();
                inst.Create();
            }
        }

        public void ConfigurOrders(in GlobalBehaviourOrders settings)
        {
            m_Settings = settings;
        }

        protected override void Create()
        {
            m_SelectedIndex = -1;
            CreateQueues();
        }

        private void Update()
        {
            if (!OrdersReady(out GlobalBehaviour[] orders)) return;
            CheckOrders(orders);
            ExecuteOrders(orders);
        }

        protected override void Destroy()
        {
            if (!OrdersReady(out GlobalBehaviour[] orders)) return;
            ExecuteDestroy(orders);
            ReleaseQueues();
            m_SelectedIndex = -1;
        }

        private bool OrdersReady(out GlobalBehaviour[] orders)
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
            m_DestroyQueue = new List<int>();
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

        private void ReleaseList(ref List<int> list)
        {
            if (list != null)
            {
                list.Clear();
                list = null;
            }
        }

        private void CheckOrders(in GlobalBehaviour[] orders)
        {
            ClearQueues();
            for (int i = 0; i < orders.Length; i++)
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

        private void ClearQueues()
        {
            m_AwakeQueue.Clear();
            m_EnableQueue.Clear();
            m_ActiveQueue.Clear();
            m_DisableQueue.Clear();
            m_DestroyQueue.Clear();
        }

        private void ExecuteOrders(in GlobalBehaviour[] orders)
        {
            ExecuteAwake(orders);
            ExecuteEnable(orders);
            ExecuteUpdate(orders);
            ExecuteDisable(orders);
            ExecuteDestroy(orders);
        }

        private void ExecuteAwake(in GlobalBehaviour[] orders)
        {
            for (int i = 0; i < orders.Length; i++)
            {
                int index = m_AwakeQueue[i];
                GlobalBehaviour behaviour = orders[index];
                behaviour.ExecuteAwake();
            }
        }

        private void ExecuteEnable(in GlobalBehaviour[] orders)
        {
            for (int i = 0; i < m_EnableQueue.Count; i++)
            {
                int index = m_EnableQueue[i];
                GlobalBehaviour behaviour = orders[index];
                behaviour.ExecuteEnable();
            }
        }

        private void ExecuteUpdate(in GlobalBehaviour[] orders)
        {
            for (int i = 0; i < m_ActiveQueue.Count; i++)
            {
                int index = m_ActiveQueue[i];
                GlobalBehaviour behaviour = orders[index];
                behaviour.ExecuteUpdate();
            }
        }

        private void ExecuteDisable(in GlobalBehaviour[] orders)
        {
            for (int i = 0; i < m_DisableQueue.Count; i++)
            {
                int index = m_DisableQueue[i];
                GlobalBehaviour behaviour = orders[index];
                behaviour.ExecuteDisable();
            }
        }

        private void ExecuteDestroy(in GlobalBehaviour[] orders)
        {
            for (int i = 0; i < orders.Length; i++)
            {
                int index = m_DestroyQueue[i];
                GlobalBehaviour behaviour = orders[index];
                behaviour.CheckExecuteDisable();
                behaviour.ExecuteDestroy();
            }
        }
    }
}