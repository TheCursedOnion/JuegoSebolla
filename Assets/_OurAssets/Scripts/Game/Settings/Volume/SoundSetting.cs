using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ami.BroAudio;
using CursedOnion.Game.CloudSave;
using Unity.Services.CloudSave.Models;
using UnityEngine;
using UnityEngine.Audio;

namespace CursedOnion.Game.Settings
{
    [Serializable]
    public class SoundSetting : ICloudStorable
    {
        public enum SoundType
        {
            SFX,
            Music,
        }
        
        [SerializeField] AudioMixer fungusMixer;
        [SerializeField] private float sfxVolume = 0.8f;
        [SerializeField] private float musicVolume = 0.8f;
        public Action OnChange { get; set; }
        public void SetSfxVolume(float volume, float fadeTime = BroAdvice.FadeTime_Immediate)
        {
            sfxVolume = volume;
            fungusMixer.SetFloat("SFX", ConvertLinearToDb(volume));
            BroAudio.SetVolume(BroAudioType.SFX, volume, fadeTime);
            
            OnChange?.Invoke();
        }
        public float GetSFXVolume() => sfxVolume;

        public void SetMusicVolume(float volume, float fadeTime = BroAdvice.FadeTime_Immediate)
        {
            musicVolume = volume;
            fungusMixer.SetFloat("Music", ConvertLinearToDb(volume));
            BroAudio.SetVolume(BroAudioType.Music, volume, fadeTime);
            
            OnChange?.Invoke();
        }
        public float GetMusicVolume() => musicVolume;
        static float ConvertLinearToDb(float linear)
        {
            return 20 * Mathf.Log10(Mathf.Clamp(linear, 0.0001f, 1f));
        }

        #region Cloud Storing
        const string SFX = "sfx";
        const string MUSIC = "music";
        const float FADE_VOLUME_TIME = 2.5f;
        public void SaveInto(Dictionary<string, object> serializableData)
        {
            serializableData[SFX] = GetSFXVolume();
            serializableData[MUSIC] = GetMusicVolume();
        }
        public void LoadFrom(Dictionary<string, Item> loadedData)
        {
            float defaultVolume = 0.5f;
            
            float sfx = CloudUtils.GetValueFromQuery(loadedData, SFX, defaultVolume);
            float music = CloudUtils.GetValueFromQuery(loadedData, MUSIC, defaultVolume);
            
            SetSfxVolume(sfx, FADE_VOLUME_TIME);
            SetMusicVolume(music, FADE_VOLUME_TIME);
        }
        #endregion
        
    }
}