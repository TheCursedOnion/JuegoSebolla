using UnityEngine;

namespace CursedOnion.Game.Systems.Files
{
    public class GameFile
    {
        public string FileName;
        public string Extension;

        public string SaveTitle;
        public string SaveMessage;
        
        public void SetFileName(string fileName) => FileName = fileName;
        public void SetSaveTitle(string saveTitle) => SaveTitle = saveTitle;
        public void SetSaveMessage(string saveMessage) => SaveMessage = saveMessage;
    }
}
