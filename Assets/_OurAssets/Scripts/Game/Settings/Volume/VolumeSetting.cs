using System;
using Ami.BroAudio;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Audio;

namespace CursedOnion.Game.Settings
{
    [Serializable]
    public class VolumeSetting : ISetting<float>
    {
        public enum SoundType
        {
            SFX,
            Music,
        }
        
        [SerializeField] AudioMixer fungusMixer;
        [SerializeField] private float sfxVolume = 0.8f;
        [SerializeField] private float musicVolume = 0.8f;
        
        public void Initialize()
        {
            sfxVolume = musicVolume = 0.8f;
            SetSfxVolume(sfxVolume);
            SetMusicVolume(musicVolume);
        }
        
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
    }
}