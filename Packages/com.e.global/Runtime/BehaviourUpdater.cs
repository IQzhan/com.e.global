using UnityEngine;

namespace E
{
    /// <summary>
    /// Update GlobalBehaviours at runtime,
    /// do not delete this gameobject.
    /// </summary>
    // Update in editor mode.
    [ExecuteAlways]
    // Hide in "Component" Menu.
    [AddComponentMenu("")]
    // Make sure only one updater.
    [Singleton(
        Name = "[Behaviour Updater]",
        Persistent = true,
        HideFlags = HideFlags.HideInInspector)]
    public sealed partial class BehaviourUpdater : Singleton<BehaviourUpdater>
    {
        private void FixedUpdate()
        {
            BehaviourManager.instance.FixedUpdate();
        }

        private void Update()
        {
            BehaviourManager.instance.Update();
        }

        private void LateUpdate()
        {
            BehaviourManager.instance.LateUpdate();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            BehaviourManager.instance.OnDrawGizmos();
        }
#endif
    }
}