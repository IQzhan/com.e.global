using System;
using UnityEngine;

namespace E
{
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

        public bool isExecuteInEditorMode;

        public bool isAutoInstantiation;

        public int order;

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

        public override bool Equals(object obj)
        {
            return obj is TypeInfo info && Equals(info);
        }

        public bool Equals(TypeInfo other)
        {
            return m_TypeHashCode == other.m_TypeHashCode &&
                   isExecuteInEditorMode == other.isExecuteInEditorMode &&
                   isAutoInstantiation == other.isAutoInstantiation &&
                   order == other.order;
        }

        public override int GetHashCode()
        {
            int hashCode = -1017475848;
            hashCode = hashCode * -1521134295 + m_TypeHashCode;
            hashCode = hashCode * -1521134295
                + (isExecuteInEditorMode ? 1 : 0)
                + (isAutoInstantiation ? 2 : 0);
            hashCode = hashCode * -1521134295 + order;
            return hashCode;
        }

        public static bool operator ==(TypeInfo left, TypeInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TypeInfo left, TypeInfo right)
        {
            return !(left == right);
        }
    }
}