using System;
using Reflex.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace CursedOnion.Game.Settings
{
    public class VolumeSlider : MonoBehaviour
    {
        [SerializeField] private Slider volumeSlider;
        [SerializeField] SoundSetting.SoundType soundType;
        SoundSetting soundSetting;

        private void Awake()
        {
            soundSetting ??= gameObject.scene.GetSceneContainer().Resolve<GameSettings>().SoundSettings;
            UpdateSlider();
        }
        private void OnEnable()
        {
            soundSetting.OnChange += UpdateSlider;
        }
        private void OnDisable()
        {
            soundSetting.OnChange -= UpdateSlider;
        }
        
        void UpdateSlider()
        {
            if (soundSetting != null)
            {
                float value = soundType switch
                {
                    SoundSetting.SoundType.SFX => soundSetting.GetSFXVolume(),
                    SoundSetting.SoundType.Music => soundSetting.GetMusicVolume(),
                    _ => throw new ArgumentOutOfRangeException()
                };
                volumeSlider.SetValueWithoutNotify(value);
            }
        }
    }
}