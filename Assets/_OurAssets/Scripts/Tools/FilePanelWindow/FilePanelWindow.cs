using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CursedOnion.Tools
{
    public struct FileAsset
    {
        public static FileAsset Default => new FileAsset { name = "New Asset" };
        
        public Object objectAsset;
        public string name;
        public string extension;

        public void SetObject(Object objectAsset, string extension)
        {
            this.objectAsset = objectAsset;
            this.extension = extension;
        }
    }
    public static class FilePanelWindow
    {
        public static void SaveFileAsset( ref FileAsset asset, string saveTitle = "Guardar Asset Nuevo", string saveMessage = "Elige la dirección para guardar tu asset")
        {
            #if UNITY_EDITOR
                string path = EditorUtility.SaveFilePanelInProject(
                    saveTitle,
                    asset.name,
                    asset.extension,
                    saveMessage
                );

                if (!string.IsNullOrEmpty(path))
                {
                    AssetDatabase.CreateAsset(asset.objectAsset, path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    Debug.Log($"Objeto {asset.name} se guardado correctamente en {path}");
                }
                else
                {
                    Debug.LogWarning("Guardado cancelado por el usuario");
                }
            #endif
        }
    }
}
