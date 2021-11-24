using UnityEngine;
using static E.BehaviourManager;

namespace E
{
    [AutoInstantiate, ExecuteAlways]
    internal class TestBehaviour0 : GlobalBehaviour
    {
        protected override bool IsActive => true;

        private GameObject obj;

        private Transform trans;

        protected override void OnAwake()
        {
            obj = GameObject.Find("Cube");
            trans = obj.transform;
            Debug.Log($"Cube: {trans.position}");
        }

        protected override void OnUpdate()
        {
            Vector3 a = new Vector3(1, 0, 0);
            Vector3 b = new Vector3(-1, 0, 0);
            float t = 0.5f * Mathf.Sin(Time.time) + 0.5f;
            Vector3 pos = a * (1 - t) + b * t;
            trans.position = pos;
        }

        protected override void OnDestroy()
        {
            
        }
    }
}