using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

namespace E
{
    [ExecuteInEditMode]
    [AutoInstantiate(5)]
    public class TestBehaviour1 : TestBehaviour1<TestBehaviour1>
    {
        protected override void OnAwake()
        {
            Debug.Log("OnAwake啦");
            BehaviourManager.OnDrawGizmosCallback -= OnDrawGizmos;
            BehaviourManager.OnDrawGizmosCallback += OnDrawGizmos;
            BehaviourManager.CreateInstance<TestBehabiour0>();
            TestBehabiour0 eatShit = BehaviourManager.GetInstance<TestBehabiour0>();
            BehaviourManager.DestroyInstance(eatShit);
        }

        private void OnDrawGizmos()
        {
            Handles.DrawLine(Vector3.zero, Vector3.one * 5);
        }

        protected override void OnEnable()
        {
            Debug.Log("OnEnable啦");
            
        }

        protected override void OnUpdate()
        {
            Debug.Log("OnUpdate啦");
        }

        protected override void OnDisable()
        {
            Debug.Log("OnDisable啦");
        }

    }

    public class TestBehaviour1<T> : GlobalBehaviour
    {
        protected override bool IsEnabled => true;
    }
}
