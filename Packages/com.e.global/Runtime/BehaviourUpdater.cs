using UnityEngine;

namespace E
{
    [ExecuteAlways, Singleton(Name = "Behaviour Updater", Persistent = true)]
    public class BehaviourUpdater : Singleton<BehaviourUpdater>
    {
        private void FixedUpdate()
        {
            BehaviourManager.Instance.FixedUpdate();
        }

        private void Update()
        {
            BehaviourManager.Instance.Update();
        }

        private void LateUpdate()
        {
            BehaviourManager.Instance.LateUpdate();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            BehaviourManager.Instance.OnDrawGizmos();
        }
#endif
    }
}