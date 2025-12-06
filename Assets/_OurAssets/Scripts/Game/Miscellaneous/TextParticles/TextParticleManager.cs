using CursedOnion.Game.Localization;
using CursedOnion.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

namespace CursedOnion.Game.Miscellaneous
{
    public class TextParticleManager
    {
        private ObjectPool<GameObject> textParticlePool;
        private GameObject textParticlePrefab;
        public TextParticleManager(GameObject textParticlePrefab)
        {
            this.textParticlePrefab = textParticlePrefab;
            textParticlePool = PoolHelper.CreatePool(CreateParticle);
        }

        GameObject CreateParticle()
        {
            GameObject go = Object.Instantiate(textParticlePrefab);
            go.GetComponent<TextParticle>().Initialize(this);

            return go;
        }

        public void ReturnParticle(GameObject particle)
        {
            textParticlePool.Release(particle);
        }
        public void SpawnTextAt(string text, Vector3 position)
        {
            var particle = textParticlePool.Get();
            
            if (particle == null || particle == false)
            {
                particle = CreateParticle();
                
                if(particle == null || particle == false) return;
            }
            
            
            
            particle.transform.position = position;
            
            var textParticle = particle.GetComponent<TextParticle>();
            textParticle.SetText(text);
            textParticle.PlayAnimation();
        }

        public void SpawnKeyTextAt(string key, Vector3 position)
        {
            var particle = textParticlePool.Get();
            particle.transform.position = position;
            
            var textParticle = particle.GetComponent<TextParticle>();
            textParticle.SetKey(key);
            textParticle.PlayAnimation();
        }
    }
}
