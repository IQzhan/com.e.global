using System;

namespace E
{
    /// <summary>
    /// Mark <see cref="GlobalBehaviour"/> auto instantiate.
    /// <para>See also: <seealso cref="TypeInfo"/></para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class AutoInstantiateAttribute : Attribute
    {
        public AutoInstantiateAttribute(int order = -1)
        {
            this.order = order;
        }

        /// <summary>
        /// Auto instantiate order,
        /// smaller,earlier.
        /// <para>See also: <seealso cref="TypeInfo.order"/></para>
        /// </summary>
        public int order;
    }
}