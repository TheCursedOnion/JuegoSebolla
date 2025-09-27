using System.IO;

namespace CursedOnion.Game.Systems.Files
{
    public class BinaryFile : GameFile
    {
        public ISaveableBinary SaveableBinary;
        public static BinaryFile DefaultFile(ISaveableBinary saveableBinary, string extension)
        {
            return new BinaryFile
            {
                FileName = "New File",
                SaveTitle = "Guardar Archivo Nuevo",
                SaveMessage = "Elige la dirección para guardar tu Archivo",
                Extension = extension,
                SaveableBinary = saveableBinary
            };
        }
        public void SetSaveableBinary(ISaveableBinary saveableBinary, string extension)
        {
            SaveableBinary = saveableBinary;
            Extension = extension;
        }
        
        public void SaveBinary()
        {
            if (FilePanelWindow.TryGetSaveBinaryPath(this, out string path) && BinarySaveSystem.TryGetWriter(path, FileMode.Create, out var writer))
            {
                SaveableBinary.SaveProcess(writer);
            }
        }

        public void LoadBinary()
        {
            if (FilePanelWindow.TryGetLoadBinaryPath(this, out string path) && BinarySaveSystem.TryGetReader(path, out var reader))
            {
                SaveableBinary.LoadProcess(reader);
            }
        }
    }
}