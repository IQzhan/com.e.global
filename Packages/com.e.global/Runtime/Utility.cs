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

        internal static unsafe uint UniqueRuntimeGlobalID()
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
                string assetsFolderStr = "Assets";
                string settingsFolderStr = "Global Settings";
                string resourcesStr = "Resources";
                if (!AssetDatabase.IsValidFolder($"{assetsFolderStr}/{settingsFolderStr}"))
                {
                    AssetDatabase.CreateFolder(assetsFolderStr, settingsFolderStr);
                }
                if (!AssetDatabase.IsValidFolder($"{assetsFolderStr}/{settingsFolderStr}/{resourcesStr}"))
                {
                    AssetDatabase.CreateFolder($"{assetsFolderStr}/{settingsFolderStr}", resourcesStr);
                }
                Editor.ScriptableObjectAssetsCreator.
                    CreateScriptableObjectAsset<T>($"{assetsFolderStr}/{settingsFolderStr}/{resourcesStr}/{typeof(T).Name}.asset");
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

        public static bool IsPlaying
        {
            get
            {
#if UNITY_EDITOR
                return EditorApplication.isPlayingOrWillChangePlaymode;
#else
                return Application.isPlaying;
#endif
            }
        }

        public static bool AllowLog { get => Debug.isDebugBuild && GlobalSettings.AllowLog; }

        public static bool AllowLogError { get => Debug.isDebugBuild && GlobalSettings.AllowLogError; }

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