using System.IO;
using UnityEditor;
using UnityEngine;

namespace E.Editor
{
    [CustomEditor(typeof(GlobalBehaviourOrders))]
    public class GlobalBehaviourOrdersEditor : UnityEditor.Editor
    {
        [MenuItem("Assets/Create/Global/Orders")]
        public static void Create()
        {
            string[] guids = Selection.assetGUIDs;
            if (guids.Length > 0)
            {
                string folderPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                if (!AssetDatabase.IsValidFolder(folderPath))
                {
                    folderPath = Path.GetDirectoryName(folderPath);
                }
                string assetPath = Path.Combine(folderPath, $"New {nameof(GlobalBehaviourOrders)}.asset");
                ScriptableObject scriptObj = CreateInstance<GlobalBehaviourOrders>();
                AssetDatabase.CreateAsset(scriptObj, assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        public static void Draw(in GlobalBehaviourOrders inst)
        {

        }
    }
}
