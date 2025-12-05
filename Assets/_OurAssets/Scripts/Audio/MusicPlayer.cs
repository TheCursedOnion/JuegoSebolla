using System;
using Ami.BroAudio;
using Ami.BroAudio.Runtime;
using CursedOnion.Game.Modes.General.UI.Events;
using CursedOnion.Locators;
using NaughtyAttributes;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Audio
{
    public enum MusicType
    {
       Menu,
       Dialog,
       Map,
       GreeceGameplay,
       EgyptGameplay,
       JapanGameplay,
    }

    public class MusicPlayer : MonoBehaviour
    {
        [Inject] RuntimeVariableLocator variableLocator;
        [Inject] UIEvents uiEvents;
        [SerializeField] private SoundID initialMenu = default;
        [SerializeField] private SoundID dialogMusic = default;

        [SerializeField] private SoundID mapMusic = default;

        [SerializeField] private SoundID greeceMusic = default;
        [SerializeField] private SoundID egyptMusic = default;
        [SerializeField] private SoundID japanMusic = default;

        [HorizontalLine] [SerializeField] private bool playOnAwake = false;
        [SerializeField] private SoundID awakeMusic = default;
        
        void Awake()
        {
            var instance = variableLocator.MusicPlayer;
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                variableLocator.MusicPlayer = this;
                DontDestroyOnLoad(gameObject);
                
                if (playOnAwake)
                {
                    PlayMusic(awakeMusic);
                }
            }
        }


        public void RequestMusic(MusicType musicType)
        {
            switch (musicType)
            {
                case MusicType.Menu: PlayMusic(initialMenu); break;
                case MusicType.Dialog: PlayMusic(dialogMusic); break;
                case MusicType.Map: PlayMusic(mapMusic); break;
                case MusicType.GreeceGameplay: PlayMusic(greeceMusic); break;
                case MusicType.EgyptGameplay: PlayMusic(egyptMusic); break;
                case MusicType.JapanGameplay: PlayMusic(japanMusic); break;
            }
        }

        void PlayMusic(SoundID soundID)
        {
            if (!SoundManager.Instance.HasAnyPlayingInstances(soundID))
                BroAudio.Play(soundID).AsBGM();
        }

        public void StopMusic()
        {
            BroAudio.Stop(BroAudioType.Music);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                PlayMusic(greeceMusic);
            }
            
            if (Input.GetKeyDown(KeyCode.J))
            {
                PlayMusic(initialMenu);
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                PlayMusic(dialogMusic);
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                PlayMusic(mapMusic);
            }
        }
    }
}
