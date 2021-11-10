using System;

namespace E
{
    [AttributeUsage(AttributeTargets.Method)]
    public class InitializeBeforeAllBehavioursMethodAttribute : Attribute
    {
        public InitializeBeforeAllBehavioursMethodAttribute() { }
    }
}