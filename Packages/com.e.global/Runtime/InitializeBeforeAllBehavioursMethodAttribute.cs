using System;

namespace E
{
    /// <summary>
    /// Make method call before auto create all <see cref="GlobalBehaviour"/>s.
    /// <para>See also:
    /// <seealso cref="BehaviourManager.ResetTypeInfosCallback"/>,
    /// <seealso cref="BehaviourManager.OverrideCreateInstanceCallback"/></para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class InitializeBeforeAllBehavioursMethodAttribute : Attribute
    {
        public InitializeBeforeAllBehavioursMethodAttribute() { }
    }
}