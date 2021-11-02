namespace E
{
    internal partial class Behaviours
    {
        private void OnDrawGizmos()
        {
            if (!m_Initialized) return;
            for (int i = 0; i < m_ActiveList.Count; i++)
            {
                int index = m_ActiveList[i];
                GlobalBehaviour behaviour = all[index];
                behaviour.ExecuteDrawGizmos(m_SelectedIndex == index);
            }
        }
    }
}