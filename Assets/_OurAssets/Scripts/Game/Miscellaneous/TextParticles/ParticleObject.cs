using UnityEngine;
using UnityEngine.Serialization;

namespace CursedOnion.Game.Miscellaneous
{
    public class ParticleObject : MonoBehaviour
    {
        [FormerlySerializedAs("particleSystem")] [SerializeField] private ParticleSystem attachedParticleSystem;
        ParticleManager particleManager;
        string particleType;
        public void Initialize(string particleType, ParticleManager particleManager)
        {
            this.particleType = particleType;
            this.particleManager = particleManager;
            attachedParticleSystem ??= GetComponent<ParticleSystem>();
        }

        public void Play()
        {
            attachedParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            attachedParticleSystem.Play();
        }

        void OnParticleSystemStopped()
        {
            particleManager.ReturnParticle(particleType, gameObject);
        }
    }
}