using UnityEngine;

namespace E
{
    [ExecuteAlways]
    public class TestBehaviour2 : GlobalBehaviour
    {
        protected override bool IsActive => true;

        protected override void OnAwake()
        {
            Debug.Log("OnAwake 2");
        }

        protected override void OnEnable()
        {
            Debug.Log("OnEnable 2");
        }

        protected override void OnUpdate()
        {
            //Debug.Log("OnUpdate 2");
        }
    }
}