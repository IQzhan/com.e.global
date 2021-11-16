using System;
using UnityEngine;
using System.Threading;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace E
{
    internal static class Utility
    {
        private static int m_GlobalIDOrder = 0;

        internal static unsafe uint UniqueGlobalID()
        {
            Interlocked.Add(ref m_GlobalIDOrder, 1);
            return (uint)m_GlobalIDOrder;
        }

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

        public static T Load<T>() where T : UnityEngine.Object
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

        public static bool IsPlaying { get => Application.isPlaying; }

        public static bool AllowLog { get => Debug.isDebugBuild; }

        public static void Log(string message)
        {
            Debug.Log(message);
        }

        public static void LogError(string message)
        {
            Debug.LogError(message);
        }

        public static void LogException(Exception e)
        {
            Debug.LogException(e);
        }
    }
}