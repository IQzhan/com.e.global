using UnityEngine;

namespace E
{
    [ExecuteInEditMode]
    public partial class TestBehabiour0 : GlobalBehaviour
    {
        protected override bool IsEnabled => true;

        protected override void OnAwake()
        {
            Debug.Log("Awake 0");
        }

        protected override void OnEnable()
        {
            Debug.Log("OnEnable 0");
        }

        protected override void OnUpdate()
        {
            Debug.Log("OnUpdate 0");
        }

        protected override void OnDisable()
        {
            Debug.Log("OnDisable 0");
        }

        protected override void OnDestroy()
        {
            Debug.Log("OnDestroy 0");
        }
    }
}