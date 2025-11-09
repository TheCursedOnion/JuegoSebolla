using System;
using UnityEngine;

namespace CursedOnion.Game.Settings
{
    [Serializable]
    public class VolumeSetting : ISetting<VolumeSetting>
    {
        [SerializeField] float sfxVolume;
        public Action<VolumeSetting> OnChange { get; set; }
        public void SetSfxVolume(float volume)
        {
            sfxVolume = volume;
            OnChange?.Invoke(this);
        }
    }
}