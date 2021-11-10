using UnityEngine;

namespace E
{
    [ExecuteInEditMode]
    public partial class TestBehabiour0 : GlobalBehaviour
    {
        protected override bool IsEnabled => true;

        protected override void OnAwake()
        {
            Debug.Log("Awake");
        }

        protected override void OnEnable()
        {
            Debug.Log("OnEnable");
        }

        protected override void OnUpdate()
        {
            Debug.Log("OnUpdate");
        }

        protected override void OnDisable()
        {
            Debug.Log("OnDisable");
        }

        protected override void OnDestroy()
        {
            Debug.Log("OnDestroy");
        }
    }
}