using UnityEngine;

namespace E
{
    /// <summary>
    /// Update global system at runtime,
    /// do not delete this gameobject.
    /// </summary>
    // Update in editor mode.
    [ExecuteAlways]
    // Hide in "Component" Menu.
    [AddComponentMenu("")]
    // Make sure only one updater.
    [Singleton(
        Name = "[Global Updater]",
        Persistent = true,
        HideFlags = HideFlags.HideInInspector)]
    public sealed partial class GlobalUpdater : Singleton<GlobalUpdater>
    {
        private void FixedUpdate()
        {
            bool allowUpdate = GlobalTime.instance.FixedUpdate();
            BehaviourManager.instance.FixedUpdate(allowUpdate);
        }

        private void Update()
        {
            bool allowUpdate = GlobalTime.instance.Update();
            BehaviourManager.instance.Update(allowUpdate);
        }

        private void LateUpdate()
        {
            bool allowUpdate = GlobalTime.instance.LateUpdate();
            BehaviourManager.instance.LateUpdate(allowUpdate);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            BehaviourManager.instance.OnDrawGizmos();
        }
#endif
    }
}