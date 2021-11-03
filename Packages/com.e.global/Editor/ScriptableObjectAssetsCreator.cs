using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace E.Editor
{
    public class ScriptableObjectAssetsCreator
    {
        [MenuItem("Assets/Create/ScriptableObject To Asset")]
        public static void CreateScriptableObjectAssets()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj is MonoScript)
            {
                MonoScript script = obj as MonoScript;
                Type cl = script.GetClass();
                if (cl != null && cl.IsSubclassOf(typeof(ScriptableObject)))
                {
                    string path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
                    path = Path.Combine(Path.GetDirectoryName(path), script.name + ".asset");
                    ScriptableObject scriptObj = ScriptableObject.CreateInstance(cl);
                    AssetDatabase.CreateAsset(scriptObj, path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    return;
                }
            }
            Debug.LogWarning("Please select a ScriptableObject script to create asset");
        }
    }

}