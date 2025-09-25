using System.IO;
using UnityEngine;

namespace CursedOnion.Game.Systems.Files
{
    public static class BinarySaveSystem
    {
        public static bool TryGetWriter(string filePath, FileMode fileMode, out BinaryWriter writer)
        {
            writer = null;

            if (string.IsNullOrEmpty(filePath))
                return false;

            try
            {
                writer = new BinaryWriter(File.Open(filePath, fileMode));
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError("No se pudo crear BinaryWriter: " + e);
                return false;
            }
        }

        public static bool TryGetReader(string filePath, out BinaryReader reader)
        {
            reader = null;
            if (File.Exists(filePath))
                reader = new BinaryReader(File.OpenRead(filePath));
                
            return reader != null;
        }
    }
}
