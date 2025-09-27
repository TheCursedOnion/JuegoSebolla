using UnityEditor;
using UnityEngine;

namespace CursedOnion.Game.Systems.Files
{
    public class AssetFile : GameFile
    {
        public Object AssetObject;
        public static AssetFile DefaultFile(Object objectAsset, string extension)
        {
            return new AssetFile
            {
                FileName = "New Asset",
                SaveTitle = "Guardar Asset Nuevo",
                SaveMessage = "Elige la dirección para guardar tu asset",
                Extension = extension,
                AssetObject = objectAsset
            };
        }
        public void SetAssetObject(Object assetObject, string extension)
        {
            AssetObject = assetObject;
            Extension = extension;
        }

        public void SaveAsset()
        {
            if (FilePanelWindow.TryGetProjectPath(this, out string path))
            {
                AssetDatabase.CreateAsset(AssetObject, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}