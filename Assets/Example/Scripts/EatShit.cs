using UnityEngine;

namespace E
{
    public partial class EatShit : GlobalBehaviour
    {
        protected override bool IsEnabled => true;

        protected override void OnAwake()
        {
            Debug.Log("Awake");
        }
    }
}