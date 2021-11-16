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

            //NativeAction<FuckMe, int, int, int> s = new NativeAction<FuckMe, int, int, int>( Unity.Collections.Allocator.Persistent);
            //s.Add(new FuckMe());
            //s.InvokeJob<FuckMe>(3, 4, 5);
            //s.Dispose();
        }

        public struct FuckMe : IAction<int, int, int>
        {
            public void Invoke(int t0, int t1, int t2)
            {
                
            }
        }


        private static void OnDrawGizmosCallback()
        {
            Handles.DrawLine(Vector3.one, Vector3.one * 2 + Vector3.forward * 3);
        }

    }
}