using System;
using Ami.BroAudio;
using CursedOnion.Game.Settings;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Audio
{
    public class MusicPlayer : MonoBehaviour
    {
        [SerializeField] private SoundID musicID = default;

        public void StartMusic()
        {
           var audioPlayer = BroAudio.Play(musicID).AsBGM();
        }
    }
}
