namespace CursedOnion.Game.Systems.Files
{
    public struct BinaryFile
    {
        public static BinaryFile Default => new BinaryFile { FileName = "New Binary File", SaveTitle = "Guardar Archivo Nuevo", SaveMessage = "Elige la dirección para guardar tu Archivo"};
        
        public ISaveableBinary SaveableBinary;
        
        public string FileName;
        public string Extension;

        public string SaveTitle;
        public string SaveMessage;

        public void SetSaveableBinary(ISaveableBinary saveableBinary, string extension)
        {
            this.SaveableBinary = saveableBinary;
            this.Extension = extension;
        }
        
        
    }
}