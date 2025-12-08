using System.Collections.Generic;
using Ami.BroAudio;
using Ami.BroAudio.Runtime;
using CursedOnion.Game.Miscellaneous;
using UnityEngine;

namespace CursedOnion.Game.Audio
{
    [System.Serializable]
    public class AudioEntry
    {
        public string AudioName;
        public SoundID AssociatedID;
    }
    
    [CreateAssetMenu(fileName = "AudioGallery", menuName = "Game/Audio/Audio Gallery")]
    public class AudioGallery : ScriptableObject
    {
        [SerializeField] private List<AudioEntry> audioEntries;
        private Dictionary<string, SoundID> audios;

        public void Initialize()
        {
            audios = new Dictionary<string, SoundID>();
            foreach (var audioEntry in audioEntries)
                audios.Add(audioEntry.AudioName, audioEntry.AssociatedID);
        }
        
        public IAudioPlayer PlaySFX(string sfxName)
        {
            return audios.TryGetValue(sfxName, out var sound) ? PlaySFX(sound) : null;
        }
        public IAudioPlayer PlaySFX(SoundID sound)
        {
            return BroAudio.Play(sound);
        }

        public IMusicPlayer PlayMusic(string musicName)
        {
            return audios.TryGetValue(musicName, out var soundID) ? PlayMusic(soundID) : null;
        }
        public IMusicPlayer PlayMusic(SoundID soundID)
        {
            return !SoundManager.Instance.HasAnyPlayingInstances(soundID) ? BroAudio.Play(soundID).AsBGM() : null;
        }

        public void StopAllMusic()
        {
            BroAudio.Stop(BroAudioType.Music);
        }
    }
    
    
}
