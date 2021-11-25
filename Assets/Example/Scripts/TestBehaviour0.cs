using UnityEngine;

namespace E
{
    [AutoInstantiate, ExecuteAlways]
    internal class TestBehaviour0 : GlobalBehaviour
    {
        protected override bool IsActive => true;

        private Transform trans;

        protected override void OnAwake()
        {
            GameObject obj = GameObject.Find("Cube");
            Debug.Log($"Cube {obj != null}");
            if (obj == null) return;
            trans = obj.transform;
        }

        protected override void OnUpdate()
        {
            if (trans == null) return;
            Vector3 a = new Vector3(1, 0, 0);
            Vector3 b = new Vector3(-1, 0, 0);
            float t = (float)(0.5d * System.Math.Sin(GlobalTime.Time) + 0.5d);
            Vector3 pos = a * (1 - t) + b * t;
            trans.position = pos;

        }

        protected override void OnDestroy()
        {

        }
    }
}