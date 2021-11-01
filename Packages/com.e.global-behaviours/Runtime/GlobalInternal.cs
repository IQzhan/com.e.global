using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace E
{
    internal class GlobalInternal
    {

//#if UNITY_EDITOR
//        [InitializeOnLoadMethod]
//        public static void InitializeEditor()
//        {
//            Debug.Log("InitializeEditor");
//            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
//            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
//        }

//        private static void OnPlayModeStateChanged(PlayModeStateChange state)
//        {
//            switch (state)
//            {
//                case PlayModeStateChange.EnteredEditMode:
//                    Debug.Log("EnteredEditMode");
//                    break;
//                case PlayModeStateChange.EnteredPlayMode:
//                    Debug.Log("EnteredPlayMode");
//                    break;
//                case PlayModeStateChange.ExitingEditMode:
//                    Debug.Log("ExitingEditMode");
//                    break;
//                case PlayModeStateChange.ExitingPlayMode:
//                    Debug.Log("ExitingPlayMode");
//                    break;
//            }
//            //Reload();
//        }

//        [UnityEditor.Callbacks.DidReloadScripts]
//        public static void InitializeReloadScripts()
//        {
//            Debug.Log("InitializeReloadScripts");
//        }

//        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
//        public static void InitializeRuntime()
//        {
//            Debug.Log("InitializeRuntime");
//        }

//#else
//        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
//        public static void InitializeRuntime()
//        {
//            Reload();
//        }
//#endif

        private static List<GlobalBehaviour> all;

        public static void Reload()
        {
            if (all == null)
            {
                Debug.Log("Fuck");
                CollectAll();
                Behaviours.Instance.Initialize(all);
            }
        }

        private static void CollectAll()
        {
            all = new List<GlobalBehaviour>();
        }
    }
}