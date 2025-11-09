using System;
using UnityEngine;

namespace CursedOnion.Game.Settings
{
    [Serializable]
    public class VolumeSetting : ISetting<float>
    {
        [SerializeField] float sfxVolume;
        public Action<float> OnChange { get; set; }
        public void SetSfxVolume(float volume)
        {
            sfxVolume = volume;
            OnChange?.Invoke(sfxVolume);
        }
    }
}