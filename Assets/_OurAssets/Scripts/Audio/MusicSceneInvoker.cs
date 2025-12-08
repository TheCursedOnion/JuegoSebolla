using Ami.BroAudio;
using CursedOnion.Locators;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Audio
{
    public class MusicSceneInvoker : MonoBehaviour
    {
        [Inject] private AudioGallery audioGallery;
        [SerializeField] private SoundID musicToPlay;
        void Awake()
        {
            audioGallery.PlayMusic(musicToPlay);
        }
    }
}