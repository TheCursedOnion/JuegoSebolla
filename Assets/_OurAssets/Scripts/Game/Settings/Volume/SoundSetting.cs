using System;
using Ami.BroAudio;
using CursedOnion.Game.CloudSave;
using UnityEngine;
using UnityEngine.Audio;

namespace CursedOnion.Game.Settings
{
    [Serializable]
    public class SoundSetting : ISetting<float>, ICloudStorable
    {
        public enum SoundType
        {
            SFX,
            Music,
        }
        
        [SerializeField] AudioMixer fungusMixer;
        [SerializeField] private float sfxVolume = 0.8f;
        [SerializeField] private float musicVolume = 0.8f;
        
        public Action<float> OnChange { get; set; }
        public void SetSfxVolume(float volume)
        {
            sfxVolume = volume;
            fungusMixer.SetFloat("SFX", ConvertLinearToDb(volume));
            BroAudio.SetVolume(BroAudioType.SFX, volume);
        }
        public float GetSFXVolume() => sfxVolume;

        public void SetMusicVolume(float volume)
        {
            musicVolume = volume;
            fungusMixer.SetFloat("Music", ConvertLinearToDb(volume));
            BroAudio.SetVolume(BroAudioType.Music, volume);
        }
        public float GetMusicVolume() => musicVolume;
        static float ConvertLinearToDb(float linear)
        {
            return 20 * Mathf.Log10(Mathf.Clamp(linear, 0.0001f, 1f));
        }

        #region Cloud Storing
        public CloudSaveClient SaveClient { get; set; }
        public async void Save()
        {
            try
            {
                await SaveClient.Save("sfx", GetSFXVolume());
                await SaveClient.Save("music", GetMusicVolume());
            }
            catch (Exception e)
            {
                Debug.LogWarning("Error al guardar: " + e);
            }
        }
        public async void Load()
        {
            try
            {
                var sfx = await SaveClient.Load<float>("sfx");
                var music = await SaveClient.Load<float>("music");
                
                SetSfxVolume(sfx);
                SetMusicVolume(music);
            }
            catch (Exception e)
            {
                Debug.LogWarning("Error al cargar: " + e);
            }
        }
        #endregion
        
    }
}