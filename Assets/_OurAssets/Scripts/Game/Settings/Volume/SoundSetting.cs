using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ami.BroAudio;
using CursedOnion.Game.CloudSave;
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
        
        const string SFX = "sfx";
        const string MUSIC = "music";
        const float FADE_VOLUME_TIME = 2.5f;
        
        [SerializeField] AudioMixer fungusMixer;
        [SerializeField] private float sfxVolume = 0.8f;
        [SerializeField] private float musicVolume = 0.8f;
        public Action OnChange { get; set; }
        
        public void Initialize()
        {
            SetSfxVolume(0f);
            SetMusicVolume(0f);
        }
        
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
            Debug.Log("Music volume: " + volume);
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
        public CloudSaveClient SaveClient { get; set; }
        public async Task Save()
        {
            try
            {
                var data = new Dictionary<string, object>
                {
                    { SFX, GetSFXVolume() },
                    { MUSIC, GetMusicVolume() }
                };
                await SaveClient.Save(data);
            }
            catch (Exception e)
            {
                Debug.LogWarning("Error al guardar: " + e);
            }
        }
        public async Task Load()
        {
            try
            {
                var sfx = await SaveClient.Load<float>(SFX);
                var music = await SaveClient.Load<float>(MUSIC);
                
                SetSfxVolume(sfx, FADE_VOLUME_TIME);
                SetMusicVolume(music, FADE_VOLUME_TIME);
            }
            catch (Exception e)
            {
                Debug.LogWarning("Error al cargar: " + e);
            }
        }
        #endregion
        
    }
}