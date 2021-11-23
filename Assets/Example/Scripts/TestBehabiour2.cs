using UnityEngine;
using static E.BehaviourManager;

namespace E
{
    [AutoInstantiate]
    [ExecuteAlways]
    public class TestBehabiour2 : GlobalBehaviour
    {
        private bool enabled = true;

        protected override bool IsEnabled => enabled;

        protected override void OnAwake()
        {
            
        }

        protected override void OnEnable()
        {

        }

        protected override void OnUpdate()
        {
            Debug.Log("OnUpdate 2");
            CreateInstance<TestBehabiour0>();
            enabled = false;
        }

    }
}