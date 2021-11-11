using UnityEngine;

namespace E
{
    [ExecuteAlways]
    [AddComponentMenu("")]
    [Singleton(
        Name = "Behaviour Updater",
        Persistent = true,
        HideFlags = HideFlags.HideInInspector)]
    public sealed partial class BehaviourUpdater : Singleton<BehaviourUpdater>
    {
        internal BehaviourManager manager;

        private void FixedUpdate()
        {
            manager?.FixedUpdate();
        }

        private void Update()
        {
            manager?.Update();
        }

        private void LateUpdate()
        {
            manager?.LateUpdate();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            manager?.OnDrawGizmos();
        }
#endif
    }
}