using Reflex.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace CursedOnion.Game.Settings
{
    public class VolumePanel : MonoBehaviour
    {
        [SerializeField] SoundSetting.SoundType soundType;
        [SerializeField] private Slider volumeSlider;
        
        SoundSetting soundSetting;
        private void OnEnable()
        {
            soundSetting ??= gameObject.scene.GetSceneContainer().Resolve<GameSettings>().SoundSettings;
            UpdateSlider();
        }

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
        void UpdateSlider()
        {
            if (soundSetting != null)
            {
                float value = soundType switch
                {
                    SoundSetting.SoundType.SFX => soundSetting.GetSFXVolume(),
                    SoundSetting.SoundType.Music => soundSetting.GetMusicVolume(),
                };
                volumeSlider.value = value;
            }
        }
    }
}
