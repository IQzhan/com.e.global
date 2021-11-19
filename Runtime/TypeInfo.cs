using System;
using UnityEngine;

namespace E
{
    /// <summary>
    /// Type info of <see cref="GlobalBehaviour"/>.
    /// could monified by <see cref="BehaviourManager.ResetTypeInfosCallback"/>.
    /// </summary>
    public struct TypeInfo : IEquatable<TypeInfo>
    {
        public TypeInfo(in Type type)
        {
            m_Type = null;
            m_TypeHashCode = 0;
            isAutoInstantiation = false;
            isExecuteInEditorMode = false;
            order = -1;
            Initialize(type);
        }

        private Type m_Type;

        private int m_TypeHashCode;

        /// <summary>
        /// Will this type of <see cref="GlobalBehaviour"/>'s life circle methods executed in editor mode?
        /// <para>Use <see cref="ExecuteAlways"/> or <see cref="ExecuteInEditMode"/> to make this happen.
        /// or monified this field by <see cref="BehaviourManager.ResetTypeInfosCallback"/>.</para>
        /// </summary>
        public bool isExecuteInEditorMode;

        /// <summary>
        /// Auto instantiate when load game?
        /// <para>Use <see cref="AutoInstantiateAttribute"/> to make this happen.
        /// or monified this field by <see cref="BehaviourManager.ResetTypeInfosCallback"/>.</para>
        /// </summary>
        public bool isAutoInstantiation;

        /// <summary>
        /// Auto instantiate order,
        /// smaller,earlier.
        /// <para><seealso cref="AutoInstantiateAttribute.order"/></para>
        /// </summary>
        public int order;

        /// <summary>
        /// <see cref="GlobalBehaviour"/>'s type.
        /// </summary>
        public Type Value { get => m_Type; set => Initialize(value); }

        public int TypeHashCode { get => m_TypeHashCode; }

        private void Initialize(Type type)
        {
            if (type == null) return;
            SetType(type);
            SetExecuteInEditorMode();
            SetAutoInstantiation();
        }

        private void SetType(Type type)
        {
            m_Type = type;
            m_TypeHashCode = m_Type.GetHashCode();
        }

        private void SetExecuteInEditorMode()
        {
            object[] attris = m_Type.GetCustomAttributes(typeof(ExecuteInEditMode), true);
            if (attris.Length == 0)
            { attris = m_Type.GetCustomAttributes(typeof(ExecuteAlways), true); }
            isExecuteInEditorMode = attris.Length > 0;
        }

        private void SetAutoInstantiation()
        {
            object[] attris = m_Type.GetCustomAttributes(typeof(AutoInstantiateAttribute), true);
            if (attris.Length == 1)
            {
                isAutoInstantiation = true;
                order = (attris[0] as AutoInstantiateAttribute).order;
            }
            else
            {
                isAutoInstantiation = false;
                order = -1;
            }
        }

        public override string ToString()
        {
            return $"type: {m_Type},{Environment.NewLine}" +
                $"isAllowInEditorMode: {isExecuteInEditorMode},{Environment.NewLine}" +
                $"isAutoInstantiation: {isAutoInstantiation},{Environment.NewLine}" +
                $"order: {order}";
        }

        public bool Equals(TypeInfo other)
        {
            return m_TypeHashCode == other.m_TypeHashCode &&
                   isExecuteInEditorMode == other.isExecuteInEditorMode &&
                   isAutoInstantiation == other.isAutoInstantiation &&
                   order == other.order;
        }

        public override bool Equals(object obj) => obj is TypeInfo info && Equals(info);

        public override int GetHashCode() => m_TypeHashCode;

        public static bool operator ==(TypeInfo left, TypeInfo right) => left.Equals(right);

        public static bool operator !=(TypeInfo left, TypeInfo right) => !(left == right);
    }
}