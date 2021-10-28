using System.Collections.Generic;
using UnityEngine;

namespace E
{
    internal class GlobalInternal
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        public static void InitializeRuntime()
        {
            Debug.Log("fuck me");
            //Reload();
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        public static void InitializeEditor()
        {
            Debug.Log("fuck");
            //Reload();
        }
#endif
        private static List<GlobalBehaviour> all;

        public static void Reload()
        {
            if (all == null)
            {
                all = new List<GlobalBehaviour>();
                Behaviours.Initialize(all);
            }
        }
    }

    internal class Behaviours : MonoBehaviour
    {
        private static Behaviours instance;

        private List<GlobalBehaviour> all;

        private List<int> actived;

        private int selectedIndex;

        public static void Initialize(List<GlobalBehaviour> all)
        {
            CreateInstance();
            instance.all = all;
            instance.actived = new List<int>();
            instance.selectedIndex = -1;
        }

        private static void CreateInstance()
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Behaviours>();
                if(instance == null)
                {
                    GameObject obj = new GameObject("Behaviours", typeof(Behaviours));
                    DontDestroyOnLoad(obj);
                }
            }
        }

        private void Update()
        {
            if (actived == null) return;
            actived.Clear();
            for (int i = 0; i < all.Count; i++)
            {
                GlobalBehaviour behaviour = all[i];
                if (behaviour.IsActive())
                {
                    actived.Add(i);
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (actived == null) return;
            for (int i = 0; i < actived.Count; i++)
            {
                int index = actived[i];
                GlobalBehaviour behaviour = all[index];
                behaviour.OnDrawGizmos(selectedIndex == index);
            }
        }

        private void OnDestroy()
        {
            actived.Clear();
            actived = null;
            for (int i = 0; i < all.Count; i++)
            {
                GlobalBehaviour behaviour = all[i];
                behaviour.Dispose();
            }
            all.Clear();
            all = null;
        }
    }
}