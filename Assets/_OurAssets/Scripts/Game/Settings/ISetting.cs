using System;

namespace CursedOnion.Game.Settings
{
    public interface IGlobalVolumeSetting
    {
        public GlobalVolume GlobalVolume { get; set; }

        public void SetGlobalVolume(GlobalVolume volume)
        {
            GlobalVolume = volume;
        }
    }
}