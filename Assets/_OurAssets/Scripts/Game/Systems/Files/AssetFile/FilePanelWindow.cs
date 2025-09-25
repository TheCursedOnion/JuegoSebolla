using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CursedOnion.Game.Systems.Files
{
    public static class FilePanelWindow
    {
        public static bool TryGetAssetDatabasePath(ref AssetFile asset, out string path)
        {
            #if UNITY_EDITOR
                path = EditorUtility.SaveFilePanelInProject(
                    asset.SaveTitle,
                    asset.AssetName,
                    asset.Extension,
                    asset.SaveMessage
                );

                return !string.IsNullOrEmpty(path);
            #endif
        }
        public static void SaveAssetFile(ref AssetFile asset)
        {
            #if UNITY_EDITOR
                string path = EditorUtility.SaveFilePanelInProject(
                    asset.SaveTitle,
                    asset.AssetName,
                    asset.Extension,
                    asset.SaveMessage
                );

                if (!string.IsNullOrEmpty(path))
                {
                    AssetDatabase.CreateAsset(asset.ObjectAsset, path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            #endif
        }

        public static void SaveBinaryFile(ref BinaryFile file)
        {
            #if UNITY_EDITOR
                if(file.SaveableBinary == null) return;
                
                string path = EditorUtility.SaveFilePanel(
                    file.SaveTitle,
                    "",
                    file.FileName,
                    file.Extension
                );

                if (BinarySaveSystem.TryGetWriter(path, FileMode.Create, out var writer))
                {
                    file.SaveableBinary.Save(writer);
                }
            #endif
        }
        
        public static void LoadBinaryFile(ref BinaryFile file)
        {
            #if UNITY_EDITOR
                    if(file.SaveableBinary == null) return;
                        
                    string path = EditorUtility.OpenFilePanel(
                        file.SaveTitle,
                        "",
                        file.Extension
                    );

                    if(BinarySaveSystem.TryGetReader(path, out var reader))
                        file.SaveableBinary.Load(reader);
            #endif
        }
    }
}
