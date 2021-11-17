using UnityEngine;

namespace E
{
    [AutoInstantiate]
    public class TestBehabiour2 : GlobalBehaviour
    {
        protected override bool IsEnabled => true;

        protected override void OnAwake()
        {
            Debug.Log("TestBehabiour2 Awake");
        }
    }
}