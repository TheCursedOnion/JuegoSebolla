using CursedOnion.Extensions;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Miscellaneous
{
    public class ParticleComponent : MonoBehaviour
    {
        [Inject] ParticleManager particleManager;
        
        private void Awake()
        {
            gameObject.InjectDependencies();
        }

        public void PlayParticle(string particleType)
        {
            particleManager.SpawnParticleAt(particleType, transform.position);
        }
        public void PlayParticle(string particleType, Transform transformPoint)
        {
            particleManager.SpawnParticleAt(particleType, transformPoint.position);
        }
        
    }
}
