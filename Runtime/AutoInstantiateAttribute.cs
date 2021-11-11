using System;

namespace E
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class AutoInstantiateAttribute : Attribute
    {
        public AutoInstantiateAttribute(int order = -1)
        {
            this.order = order;
        }

        public int order;
    }
}