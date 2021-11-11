using System;

namespace E
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class InitializeBeforeAllBehavioursMethodAttribute : Attribute
    {
        public InitializeBeforeAllBehavioursMethodAttribute() { }
    }
}