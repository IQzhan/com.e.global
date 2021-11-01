using System.Collections.Generic;
using UnityEngine;

namespace E
{
    [Singleton(Name = "Behaviours", Persistent = true)]
    [AddComponentMenu("")]
    [ExecuteAlways]
    public class Behaviours : Singleton<Behaviours>
    {
        private List<GlobalBehaviour> all;

        private List<int> enableList;

        private List<int> activeList;

        private List<int> disableList;

        private int selectedIndex;

        protected override void Create()
        {

        }

        protected override void Destroy()
        {
            ReleaseLists();
        }

        public void Initialize(List<GlobalBehaviour> all)
        {
            ReleaseLists();
            this.all = all;
            activeList = new List<int>();
            enableList = new List<int>();
            disableList = new List<int>();
            selectedIndex = -1;
            for (int i = 0; i < all.Count; i++)
            {
                GlobalBehaviour behaviour = all[i];
                behaviour.Setup();
            }
        }

        private void Update()
        {
            if (CheckNull()) return;
            Check();
            ExecuteEnable();
            ExecuteUpdate();
            ExecuteDisable();
        }

        private void Check()
        {
            enableList.Clear();
            activeList.Clear();
            disableList.Clear();
            for (int i = 0; i < all.Count; i++)
            {
                GlobalBehaviour behaviour = all[i];
                behaviour.Check(
                    out bool enableState,
                    out bool activeState,
                    out bool disableState);
                if (enableState) enableList.Add(i);
                if (activeState) activeList.Add(i);
                if (disableState) disableList.Add(i);
            }
        }

        private void ExecuteEnable()
        {
            for (int i = 0; i < enableList.Count; i++)
            {
                int index = enableList[i];
                GlobalBehaviour behaviour = all[index];
                behaviour.ExecuteEnable();
            }
        }

        private void ExecuteUpdate()
        {
            for (int i = 0; i < activeList.Count; i++)
            {
                int index = activeList[i];
                GlobalBehaviour behaviour = all[index];
                behaviour.ExecuteUpdate();
            }
        }

        private void ExecuteDisable()
        {
            for (int i = 0; i < disableList.Count; i++)
            {
                int index = disableList[i];
                GlobalBehaviour behaviour = all[index];
                behaviour.ExecuteDisable();
            }
        }

        private void OnDrawGizmos()
        {
            if (CheckNull()) return;
            for (int i = 0; i < activeList.Count; i++)
            {
                int index = activeList[i];
                GlobalBehaviour behaviour = all[index];
                behaviour.ExecuteDrawGizmos(selectedIndex == index);
            }
        }

        private bool CheckNull()
        {
            return all == null ||
                enableList == null ||
                activeList == null ||
                disableList == null;
        }

        private void ReleaseLists()
        {
            ReleaseList(ref enableList);
            ReleaseList(ref activeList);
            ReleaseList(ref disableList);
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
    }
}