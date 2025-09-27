using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CursedOnion.Game.Systems.Files
{
    public static class FilePanelWindow
    {
        public static bool TryGetProjectPath(AssetFile asset, out string path)
        {
            path = string.Empty;
            
            #if UNITY_EDITOR
                path = EditorUtility.SaveFilePanelInProject(
                    asset.SaveTitle,
                    asset.FileName,
                    asset.Extension,
                    asset.SaveMessage
                );
            #endif
            return !string.IsNullOrEmpty(path);
        }
        

        public static bool TryGetSaveBinaryPath(BinaryFile file, out string path)
        {
            path = string.Empty;
            
            #if UNITY_EDITOR
                path = EditorUtility.SaveFilePanel(
                    file.SaveTitle,
                    "",
                    file.FileName,
                    file.Extension
                );
            #endif
            return !string.IsNullOrEmpty(path);
        }
        
        public static bool TryGetLoadBinaryPath(BinaryFile file, out string path)
        {
            path = string.Empty;
            
            #if UNITY_EDITOR
                path = EditorUtility.OpenFilePanel(
                    file.SaveTitle,
                    "",
                    file.Extension
                );
            #endif

            return !string.IsNullOrEmpty(path);
        }
    }
}
