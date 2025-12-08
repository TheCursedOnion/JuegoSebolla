using System.Collections;
using System.Runtime.InteropServices.ComTypes;
using CursedOnion.Extensions;
using Reflex.Attributes;
using Unity.VisualScripting;
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

        public void PlayParticleWithDelay(string particleType, float delay)
        {
            StartCoroutine(IEInvokeParticleDelayed(particleType, delay));
        }

        IEnumerator IEInvokeParticleDelayed(string particleType, float delay)
        {
            yield return new WaitForSeconds(delay);
            PlayParticle(particleType);
        }
    }
}
