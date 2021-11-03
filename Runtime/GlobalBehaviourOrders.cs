using UnityEngine;

namespace E
{
    public class GlobalBehaviourOrders : ScriptableObject
    {
        [SerializeField]
        private GlobalBehaviour[] orders;

        public GlobalBehaviour[] Orders
        {
            get
            {
                if (orders == null)
                {
                    orders = new GlobalBehaviour[0];
                }
                return orders;
            }
        }
    }
}