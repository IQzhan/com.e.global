using UnityEngine;

namespace E
{
    [ExecuteAlways]
    [AutoInstantiate]
    public class TestBehaviour4 : GlobalBehaviour
    {
        protected override bool IsEnabled => true;

        protected override void OnUpdate()
        {
            //Debug.Log("Update 4");
        }
    }
}
