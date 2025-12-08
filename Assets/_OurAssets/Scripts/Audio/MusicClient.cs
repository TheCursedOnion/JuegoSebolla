using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Audio
{
    public class MusicClient : MonoBehaviour
    {
        [Inject] public AudioGallery AudioGallery;
        
        [SerializeField] private string musicAtStart;
        void Start() => AudioGallery?.PlayMusic(musicAtStart);
    }
}