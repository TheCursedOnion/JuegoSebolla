using System;

namespace CursedOnion.Game.Settings
{
    public interface ISetting<T>
    {
        public Action<T> OnChange {get; set;}
    }

    public interface IGlobalVolumeSetting<T> : ISetting<T>
    {
        public GlobalVolume GlobalVolume { get; set; }
        public void SetGlobalVolume(GlobalVolume volume);
    }
}