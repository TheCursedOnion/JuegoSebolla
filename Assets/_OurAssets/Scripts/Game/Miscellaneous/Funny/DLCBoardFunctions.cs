using System;
using Ami.BroAudio;
using Ami.BroAudio.Runtime;
using UnityEngine;

namespace CursedOnion.Game.Miscellaneous
{
    public class DLCBoardFunctions : MonoBehaviour
    {
        [SerializeField] private SoundID devilDetailsWinSound;
        
        public void CrashGame()
        {
            Application.Quit();
        }

        public void PlayFunnySound()
        {
            if(!SoundManager.Instance.HasAnyPlayingInstances(devilDetailsWinSound)) BroAudio.Play(devilDetailsWinSound);
        }
    }
}
