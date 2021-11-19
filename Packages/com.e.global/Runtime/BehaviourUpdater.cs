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
        internal BehaviourManager manager;

        protected override void Create()
        {
            // Make sure manager is not null when open the editor.
            manager = BehaviourManager.m_Instance;
        }

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