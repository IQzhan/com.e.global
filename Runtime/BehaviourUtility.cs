using UnityEngine;

namespace E
{
    internal class BehaviourUtility
    {
        public const string SettingsName = "BehaviourSettings";

        public static T Load<T>(string name) where T : Object
        {
            return Resources.Load<T>(name);
        }
    }
}