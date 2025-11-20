using CursedOnion.Game.CloudSave;

namespace CursedOnion.Game.CloudSave
{
    public interface ICloudStorable
    {
        public CloudSaveClient SaveClient {get; set;}
        
        public void Save();
        public void Load();
    }
    
}