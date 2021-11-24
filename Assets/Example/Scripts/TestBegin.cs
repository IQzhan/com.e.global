using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace E
{
    public class TestBegin
    {
        [InitializeBeforeAllBehavioursMethod]
        public static void Begin()
        {
#if UNITY_EDITOR
            BehaviourManager.OnDrawGizmosCallback += OnDrawGizmosCallback;
#endif
        }

#if UNITY_EDITOR
        private static void OnDrawGizmosCallback()
        {
            Handles.Label(Vector3.zero, "Draw Gizmos");
        }
#endif

    }
}