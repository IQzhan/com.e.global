using UnityEngine;

namespace E
{
    [ExecuteAlways]
    public class BehaviourUpdater : Singleton<BehaviourUpdater>
    {
        private void FixedUpdate()
        {
            BehaviourManager manager = BehaviourManager.Instance;
            if (manager.IsReady && manager.UpdateMethod == BehaviourSettings.UpdateMethod.FixedUpdate)
            {
                manager.Update();
            }
        }

        private void Update()
        {
            BehaviourManager manager = BehaviourManager.Instance;
            if (manager.IsReady && manager.UpdateMethod == BehaviourSettings.UpdateMethod.Update)
            {
                manager.Update();
            }
        }

        private void LateUpdate()
        {
            BehaviourManager manager = BehaviourManager.Instance;
            if (manager.IsReady && manager.UpdateMethod == BehaviourSettings.UpdateMethod.LateUpdate)
            {
                manager.Update();
            }
        }
    }
}