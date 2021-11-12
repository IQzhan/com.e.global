using System;
using UnityEditor;
using UnityEngine;

namespace E
{
    public class TestBegin
    {
        [InitializeBeforeAllBehavioursMethod]
        public static void Begin()
        {
            Debug.Log("Begin");
            BehaviourManager.OnDrawGizmosCallback += OnDrawGizmosCallback;
            //TestCreateInstance testCreateInstance = Activator.CreateInstance(typeof(TestCreateInstance), false) as TestCreateInstance;
            //TestCreateInstance a = new TestCreateInstance();
        }

        private static void OnDrawGizmosCallback()
        {
            Handles.DrawLine(Vector3.one, Vector3.one * 2 + Vector3.forward * 3);
        }

        private class InstBase
        {
            public InstBase() { }
        }
        
        private class TestCreateInstance : InstBase
        {
            //private TestCreateInstance()
            //{
            //    Debug.LogError("TestCreateInstance");
            //}

            public TestCreateInstance(int a, int b)
            {

            }
        }
    }
}