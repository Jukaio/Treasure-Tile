using UnityEditor.Presets;
using UnityEngine;
using UnityEditor;

class CreateHealth
{
    [MenuItem("Assets/Create/Health", priority = 358)]
    internal static void CreateNewTile()
    {
        string message = string.Format("Save Health Object'{0}':", "health");
        string newAssetPath = EditorUtility.SaveFilePanelInProject("Save Health Object", "New Health", "asset", message);

        if (string.IsNullOrEmpty(newAssetPath))
            return;

        AssetDatabase.CreateAsset(CreateDefaultTile(), newAssetPath);
    }

    /// <summary>Creates a Tile with defaults based on the Tile preset</summary>
    /// <returns>A Tile with defaults based on the Tile preset</returns>
    public static Health CreateDefaultTile()
    {
        return ObjectFactory.CreateInstance<Health>();
    }
}

