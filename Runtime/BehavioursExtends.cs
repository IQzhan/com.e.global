using System.Collections.Generic;

namespace E
{
    public partial class Behaviours
    {
        private void OnDrawGizmos()
        {
            if (!OrdersReady(out List<GlobalBehaviour> orders)) return;
            for (int i = 0; i < m_ActiveQueue.Count; i++)
            {
                int index = m_ActiveQueue[i];
                GlobalBehaviour behaviour = orders[index];
                behaviour.ExecuteDrawGizmos(m_SelectedIndex == index);
            }
        }
    }
}