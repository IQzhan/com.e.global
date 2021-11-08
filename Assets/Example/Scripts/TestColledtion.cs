using UnityEngine;

namespace E
{
    public class TestColledtion : MonoBehaviour
    {
        private void Start()
        {
            BehaviourCollection behaviours = new BehaviourCollection();
            EatShit eatShit = new EatShit();
            behaviours.Add(eatShit);
            EatShit eatShit1 = new EatShit();
            behaviours.Add(eatShit1);
            Fucker fucker = new Fucker();
            behaviours.Add(fucker);
            GlobalBehaviour sFucker = behaviours.Get(typeof(Fucker));
            Debug.Log(sFucker.ID);

            GlobalBehaviour[] bbs = behaviours.Gets(typeof(EatShit));
            Debug.Log(bbs.Length);
            //foreach (GlobalBehaviour behaviour in behaviours)
            //{
            //    Debug.Log(behaviour.ID);
            //}
        }
    }
}