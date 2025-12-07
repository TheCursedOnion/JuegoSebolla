using System;
using Reflex.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace CursedOnion.Game.Settings
{
    public class VolumeController : MonoBehaviour
    {
        [SerializeField] SoundSetting.SoundType soundType;
        [SerializeField] Slider volumeSlider;
        
        SoundSetting soundSetting;
        private void OnEnable()
        {
            soundSetting ??= gameObject.scene.GetSceneContainer().Resolve<GameSettings>().SoundSettings;

            soundSetting.OnChange += SetSliderValue;
            SetSliderValue();
        }
        private void OnDisable() => soundSetting.OnChange -= SetSliderValue;

        public void SetVolume()
        {
            if (soundSetting == null) return;
            
            float value = volumeSlider.value;
            switch (soundType)
            {
                case SoundSetting.SoundType.SFX: soundSetting.SetSfxVolume(value); break;
                case SoundSetting.SoundType.Music: soundSetting.SetMusicVolume(value); break;
            }
        }

        void SetSliderValue()
        {
            float value = soundType switch
            {
                SoundSetting.SoundType.SFX => soundSetting.GetSFXVolume(),
                SoundSetting.SoundType.Music => soundSetting.GetMusicVolume(),
                _ => 0
            };
            volumeSlider.SetValueWithoutNotify(value);
        }
        
    }
}
