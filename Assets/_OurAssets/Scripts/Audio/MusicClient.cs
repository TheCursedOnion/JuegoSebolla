using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Audio
{
    public class MusicClient : MonoBehaviour
    {
        [Inject] public MusicPlayer MusicPlayer { get; set; }
        
        [SerializeField] private MusicType musicType;
        void Start() => MusicPlayer?.RequestMusic(musicType);
    }
}