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
            textParticlePool = PoolHelper.CreatePool(CreateParticle, 100);
        }

        GameObject CreateParticle()
        {
            if (textParticlePrefab == null)
            {
                Debug.LogError("TextParticle prefab is NULL!");
                return null;
            }

            GameObject go = Object.Instantiate(textParticlePrefab);
            go.GetComponent<TextParticle>().Initialize(this);
            return go;
        }

        public void ReturnParticle(GameObject particle)
        {
            if (particle == null) return;
            textParticlePool.Release(particle);
        }

        GameObject GetValidParticle()
        {
            GameObject p = textParticlePool.Get();
            
            if (p == null)
            {
                p = CreateParticle();
            }

            return p;
        }

        public void SpawnTextAt(string text, Vector3 position)
        {
            var particle = GetValidParticle();
            if (particle == null) return;

            particle.transform.position = position;
            var tp = particle.GetComponent<TextParticle>();
            tp.SetText(text);
            tp.PlayAnimation();
        }

        public void SpawnKeyTextAt(string key, Vector3 position)
        {
            var particle = GetValidParticle();
            if (particle == null) return;

            particle.transform.position = position;
            var tp = particle.GetComponent<TextParticle>();
            tp.SetKey(key);
            tp.PlayAnimation();
        }
    }
}
