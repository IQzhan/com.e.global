using UnityEditor;
using UnityEngine;

namespace E
{
    public class TestBegin
    {
        [InitializeBeforeAllBehavioursMethod]
        public static void Begin()
        {
            //Debug.Log("Begin");
            BehaviourManager.OnDrawGizmosCallback += OnDrawGizmosCallback;


        }


        private static void OnDrawGizmosCallback()
        {
            Handles.DrawLine(Vector3.one, Vector3.one * 2 + Vector3.forward * 3);
        }

    }
}