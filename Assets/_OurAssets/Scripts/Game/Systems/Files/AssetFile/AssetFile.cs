using UnityEditor;
using UnityEngine;

namespace CursedOnion.Game.Systems.Files
{
    public struct AssetFile
    {
        public static AssetFile Default => new AssetFile { AssetName = "New Asset", SaveTitle = "Guardar Asset Nuevo", SaveMessage = "Elige la dirección para guardar tu asset"};
        
        public Object ObjectAsset;
        
        public string AssetName;
        public string Extension;

        public string SaveTitle;
        public string SaveMessage;

        public void SetObject(Object objectAsset, string extension)
        {
            this.ObjectAsset = objectAsset;
            this.Extension = extension;
        }

        public void SaveAsset(string path)
        {
            AssetDatabase.CreateAsset(ObjectAsset, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}