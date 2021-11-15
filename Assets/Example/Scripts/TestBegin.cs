using System;
using System.Reflection;
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
            Debug.Log($"{uint.MaxValue} {int.MaxValue}");
        }

        private static void OnDrawGizmosCallback()
        {
            Handles.DrawLine(Vector3.one, Vector3.one * 2 + Vector3.forward * 3);
        }

    }
}