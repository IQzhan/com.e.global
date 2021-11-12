using System;
using UnityEngine;

namespace E
{
    public struct TypeInfo
    {
        public TypeInfo(in Type type)
        {
            this.type = type;
            typeHashCode = type.GetHashCode();
            object[] attris = type.GetCustomAttributes(typeof(ExecuteInEditMode), true);
            if (attris.Length == 0)
            { attris = type.GetCustomAttributes(typeof(ExecuteAlways), true); }
            isExecuteInEditorMode = attris.Length > 0;
            attris = type.GetCustomAttributes(typeof(AutoInstantiateAttribute), true);
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

        public Type type;

        internal int typeHashCode;

        public bool isExecuteInEditorMode;

        public bool isAutoInstantiation;

        public int order;

        public override string ToString()
        {
            return $"type: {type},{Environment.NewLine}" +
                $"isAllowInEditorMode: {isExecuteInEditorMode},{Environment.NewLine}" +
                $"isAutoInstantiation: {isAutoInstantiation},{Environment.NewLine}" +
                $"order: {order}";
        }
    }
}