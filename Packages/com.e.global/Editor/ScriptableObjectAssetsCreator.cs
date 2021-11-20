using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace E.Editor
{
    public static class ScriptableObjectAssetsCreator
    {
        [MenuItem("Assets/Create/ScriptableObject To Asset")]
        public static void CreateScriptableObjectAsset()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj is MonoScript)
            {
                MonoScript script = obj as MonoScript;
                Type cl = script.GetClass();
                if (cl != null && cl.IsSubclassOf(typeof(ScriptableObject)))
                {
                    string path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
                    CreateScriptableObjectAsset(cl, $"{Path.GetDirectoryName(path)}/{script.name}.asset");
                    return;
                }
            }
            Debug.LogWarning("Please select a ScriptableObject script to create asset");
        }

        public static void CreateScriptableObjectAsset<T>(string path) where T : ScriptableObject
        {
            ScriptableObject scriptObj = ScriptableObject.CreateInstance<T>();
            CreateObjectAsset(scriptObj, path);
        }

        public static void CreateScriptableObjectAsset(Type type, string path)
        {
            ScriptableObject scriptObj = ScriptableObject.CreateInstance(type);
            CreateObjectAsset(scriptObj, path);
        }

        public static void CreateObjectAsset(UnityEngine.Object obj, string path)
        {
            AssetDatabase.CreateAsset(obj, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}