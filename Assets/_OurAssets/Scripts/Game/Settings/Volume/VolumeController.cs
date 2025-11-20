using System;
using Reflex.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace CursedOnion.Game.Settings
{
    public class VolumeController : MonoBehaviour
    {
        [SerializeField] SoundSetting.SoundType soundType;
        
        SoundSetting soundSetting;
        private void OnEnable()
        {
            soundSetting ??= gameObject.scene.GetSceneContainer().Resolve<GameSettings>().SoundSettings;
        }

        public void SetVolume(Slider volumeSlider)
        {
            if (soundSetting == null) return;
            
            float value = volumeSlider.value;
            switch (soundType)
            {
                case SoundSetting.SoundType.SFX: soundSetting.SetSfxVolume(value); break;
                case SoundSetting.SoundType.Music: soundSetting.SetMusicVolume(value); break;
            }
        }
        
    }
}
