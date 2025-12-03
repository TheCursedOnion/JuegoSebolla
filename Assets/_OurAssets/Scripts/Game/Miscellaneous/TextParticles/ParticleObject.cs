using UnityEngine;

namespace CursedOnion.Game.Miscellaneous
{
    public class ParticleObject : MonoBehaviour
    {
        [SerializeField] private ParticleSystem particleSystem;
        ParticleManager particleManager;
        string particleType;
        public void Initialize(string particleType, ParticleManager particleManager)
        {
            this.particleType = particleType;
            this.particleManager = particleManager;
            particleSystem ??= GetComponent<ParticleSystem>();
        }

        public void Play()
        {
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            particleSystem.Play();
        }

        void OnParticleSystemStopped()
        {
            particleManager.ReturnParticle(particleType, gameObject);
        }
    }
}