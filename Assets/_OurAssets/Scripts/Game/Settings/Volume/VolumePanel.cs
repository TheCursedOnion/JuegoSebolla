using Reflex.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace CursedOnion.Game.Settings
{
    public class VolumePanel : MonoBehaviour
    {
        [SerializeField] VolumeSetting.SoundType soundType;
        [SerializeField] private Slider volumeSlider;
        
        VolumeSetting volumeSetting;
        private void OnEnable()
        {
            volumeSetting ??= gameObject.scene.GetSceneContainer().Resolve<GameSettings>().VolumeSettings;
            UpdateSlider();
        }

        public void SetVolume()
        {
            if (volumeSetting == null) return;
            
            float value = volumeSlider.value;
            switch (soundType)
            {
                case VolumeSetting.SoundType.SFX: volumeSetting.SetSfxVolume(value); break;
                case VolumeSetting.SoundType.Music: volumeSetting.SetMusicVolume(value); break;
            }
        }
        void UpdateSlider()
        {
            if (volumeSetting != null)
            {
                float value = soundType switch
                {
                    VolumeSetting.SoundType.SFX => volumeSetting.GetSFXVolume(),
                    VolumeSetting.SoundType.Music => volumeSetting.GetMusicVolume(),
                };
                volumeSlider.value = value;
            }
        }
    }
}
