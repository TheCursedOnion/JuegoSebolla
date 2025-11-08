using UnityEngine;

namespace CursedOnion.Game.Systems.Files
{
    public class GameFile
    {
        public string FileName;
        public string Extension;

        public string SaveTitle;
        public string SaveMessage;
        
        public string DefaultPath;
        
        public void SetFileName(string fileName) => FileName = fileName;
        public void SetExtension(string extension) => Extension = extension;
        public void SetSaveTitle(string saveTitle) => SaveTitle = saveTitle;
        public void SetSaveMessage(string saveMessage) => SaveMessage = saveMessage;
        public void SetDefaultPath(string defaultPath) => DefaultPath = defaultPath;
    }
}
