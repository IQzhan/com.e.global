using UnityEngine;

namespace E
{
    [ExecuteInEditMode, AutoInstantiate(0)]
    public class TestBehaviour1 : GlobalBehaviour
    {
        protected override bool IsActive => true;

        protected override void OnAwake()
        {
            BehaviourManager.CreateInstance<TestBehaviour2>();
            Debug.Log("OnAwake 1");
        }

        protected override void OnEnable()
        {
            Debug.Log("OnEnable 1");
        }

        protected override void OnUpdate()
        {
            Debug.Log("OnUpdate 1");
        }

        protected override void OnDisable()
        {
            Debug.Log("OnDisable 1");
        }

        protected override void OnDestroy()
        {
            Debug.Log("OnDestroy 1");
        }
    }
}
