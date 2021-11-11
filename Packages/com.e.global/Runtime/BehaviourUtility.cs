using UnityEditor;
#if UNITY_EDITOR
using UnityEngine;
#endif

namespace E
{
    internal class BehaviourUtility
    {
#if UNITY_EDITOR
        public static void CreateAssetIfNotExists<T>() where T : ScriptableObject
        {
            string[] guids = AssetDatabase.FindAssets($"t: {typeof(T).FullName}");
            if (guids.Length == 0)
            {
                T inst = ScriptableObject.CreateInstance<T>();
                if (!AssetDatabase.IsValidFolder("Assets/Global Settings"))
                {
                    AssetDatabase.CreateFolder("Assets", "Global Settings");
                }
                if (!AssetDatabase.IsValidFolder("Assets/Global Settings/Resources"))
                {
                    AssetDatabase.CreateFolder("Assets/Global Settings", "Resources");
                }
                AssetDatabase.CreateAsset(inst, $"Assets/Global Settings/Resources/{typeof(T).Name}.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
#endif

        public static T Load<T>() where T : Object
        {
#if UNITY_EDITOR
            string[] guids = AssetDatabase.FindAssets($"t: {typeof(T).FullName}");
            if (guids.Length > 0)
            {
                return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guids[0]));
            }
            return default;
#else
            return Resources.Load<T>(typeof(T).Name);
#endif
        }
    }
}