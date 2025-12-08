using Ami.BroAudio;
using UnityEngine;

namespace CursedOnion.Game.Miscellaneous
{
    public class EntityAudioInvoker : MonoBehaviour
    {
        [SerializeField] private SoundID healID;
        [SerializeField] private SoundID buffID;
        
        [SerializeField] private SoundID hurtID;
        [SerializeField] private SoundID deathID;

        public void PlayHurtSound()
        {
            BroAudio.Play(hurtID);
        }

        public void PlayDeathSound()
        {
            BroAudio.Play(deathID);
        }
        
        public void PlayHealSound()
        {
            BroAudio.Play(healID);
        }
        
        public void PlayBuffSound()
        {
            BroAudio.Play(buffID);
        }
    }
}