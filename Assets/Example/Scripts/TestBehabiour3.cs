using UnityEngine;

namespace E
{
    //[AutoInstantiate]
    [ExecuteInEditMode]
    public class TestBehabiour3 : GlobalBehaviour
    {
        protected override bool IsEnabled => true;

        protected override void OnUpdate()
        {
            //Debug.Log("Update 3");
        }
    }
}