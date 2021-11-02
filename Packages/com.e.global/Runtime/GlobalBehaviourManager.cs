using System;
using System.Collections.Generic;

namespace E
{
    public partial class GlobalBehaviour
    {
        public static T AddBehaviour<T>() where T : GlobalBehaviour
        {
            return AddBehaviour(typeof(T)) as T;
        }

        public static GlobalBehaviour AddBehaviour(Type type)
        {
            return null;
        }

        public static T GetBehaviour<T>() where T : GlobalBehaviour
        {
            return GetBehaviour(typeof(T)) as T;
        }

        public static GlobalBehaviour GetBehaviour(Type type)
        {
            return null;
        }

        public static T[] GetBehaviours<T>() where T : GlobalBehaviour
        {
            return GetBehaviours(typeof(T)) as T[];
        }

        public static GlobalBehaviour[] GetBehaviours(Type type)
        {
            return null;
        }

        public static GlobalBehaviour[] GetBehaviours()
        {
            return null;
        }

        public static void RemoveBehaviour<T>(T behaviour) where T : GlobalBehaviour
        {
            
        }


    }
}