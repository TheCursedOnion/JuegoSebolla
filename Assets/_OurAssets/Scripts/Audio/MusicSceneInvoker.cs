using Ami.BroAudio;
using CursedOnion.Locators;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Audio
{
    public class MusicSceneInvoker : MonoBehaviour
    {
        [Inject] private RuntimeVariableLocator variableLocator;
        [SerializeField] private SoundID musicToPlay;
        void Awake()
        {
            variableLocator?.MusicPlayer.RequestMusic(musicToPlay);
        }
    }
}