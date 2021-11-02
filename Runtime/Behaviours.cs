using System;
using System.Collections.Generic;
using UnityEngine;

namespace E
{
    [Singleton(Name = "Behaviours", Persistent = true)]
    [AddComponentMenu("")]
    [ExecuteAlways]
    internal partial class Behaviours : Singleton<Behaviours>
    {
        private bool m_Initialized;

        private List<GlobalBehaviour> all;

        private List<int> m_InitList;

        private List<int> m_EnableList;

        private List<int> m_ActiveList;

        private List<int> m_DisableList;

        private List<int> m_DestroyList;

        private int m_SelectedIndex;

        private void InitLists()
        {
            m_InitList = new List<int>();
            m_ActiveList = new List<int>();
            m_EnableList = new List<int>();
            m_DisableList = new List<int>();
            m_DestroyList = new List<int>();
        }

        private void ClearLists()
        {
            m_InitList.Clear();
            m_EnableList.Clear();
            m_ActiveList.Clear();
            m_DisableList.Clear();
            m_DestroyList.Clear();
        }

        private void ReleaseLists()
        {
            ReleaseList(ref m_InitList);
            ReleaseList(ref m_EnableList);
            ReleaseList(ref m_ActiveList);
            ReleaseList(ref m_DisableList);
            ReleaseList(ref m_DestroyList);
            ReleaseAll();
        }

        private void ReleaseList(ref List<int> list)
        {
            if (list != null)
            {
                list.Clear();
                list = null;
            }
        }

        private void ReleaseAll()
        {
            if (all != null)
            {
                for (int i = 0; i < all.Count; i++)
                {
                    GlobalBehaviour behaviour = all[i];
                    behaviour.CheckExecuteDisable();
                    behaviour.Dispose();
                }
                all.Clear();
                all = null;
            }
        }

        protected override void Create()
        {
            ReleaseLists();
            InitLists();
            m_SelectedIndex = -1;
            m_Initialized = true;
        }

        protected override void Destroy()
        {
            ReleaseLists();
        }

        private void Update()
        {
            if (!m_Initialized) return;
            Check();
            ExecuteLife();
        }

        private void Check()
        {
            ClearLists();
            for (int i = 0; i < all.Count; i++)
            {
                GlobalBehaviour behaviour = all[i];
                behaviour.Check(
                    out bool enableState,
                    out bool activeState,
                    out bool disableState);
                if (enableState) m_EnableList.Add(i);
                if (activeState) m_ActiveList.Add(i);
                if (disableState) m_DisableList.Add(i);
            }
        }

        private void ExecuteLife()
        {
            ExecuteEnable();
            ExecuteUpdate();
            ExecuteDisable();
        }

        private void ExecuteEnable()
        {
            for (int i = 0; i < m_EnableList.Count; i++)
            {
                int index = m_EnableList[i];
                GlobalBehaviour behaviour = all[index];
                behaviour.ExecuteEnable();
            }
        }

        private void ExecuteUpdate()
        {
            for (int i = 0; i < m_ActiveList.Count; i++)
            {
                int index = m_ActiveList[i];
                GlobalBehaviour behaviour = all[index];
                behaviour.ExecuteUpdate();
            }
        }

        private void ExecuteDisable()
        {
            for (int i = 0; i < m_DisableList.Count; i++)
            {
                int index = m_DisableList[i];
                GlobalBehaviour behaviour = all[index];
                behaviour.ExecuteDisable();
            }
        }
    }
}