using UnityEditor.Presets;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEditor.Tilemaps
{
    public class CreateTile3D
    {
        [MenuItem("Assets/Create/Tile 3D", priority = 358)]
        internal static void CreateNewTile()
        {
            string message = string.Format("Save tile'{0}':", "tile3D");
            string newAssetPath = EditorUtility.SaveFilePanelInProject("Save 3D tile", "New Tile 3D", "asset", message);
            
            // If user canceled or save path is invalid, we can't create the tile
            if (string.IsNullOrEmpty(newAssetPath))
                return;

            AssetDatabase.CreateAsset(CreateDefaultTile(), newAssetPath);
        }

        /// <summary>Creates a Tile with defaults based on the Tile preset</summary>
        /// <returns>A Tile with defaults based on the Tile preset</returns>
        public static Tile3D CreateDefaultTile()
        {
            return ObjectFactory.CreateInstance<Tile3D>();
        }
    }
}
