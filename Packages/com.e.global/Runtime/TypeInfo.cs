using System;
using UnityEngine;

namespace E
{
    public struct TypeInfo
    {
        public TypeInfo(in Type type)
        {
            this.type = type;
            object[] attris = type.GetCustomAttributes(typeof(ExecuteInEditMode), true);
            if (attris.Length == 0)
            {
                attris = type.GetCustomAttributes(typeof(ExecuteAlways), true);
            }
            if (attris.Length == 1)
            {
                isAllowInEditorMode = true;
            }
            else
            {
                isAllowInEditorMode = false;
            }
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

        public bool isAllowInEditorMode;

        public bool isAutoInstantiation;

        public int order;
    }
}