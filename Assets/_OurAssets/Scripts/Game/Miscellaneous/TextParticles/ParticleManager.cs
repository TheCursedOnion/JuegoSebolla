using System.Collections.Generic;
using System.Linq;
using CursedOnion.Helpers;
using UnityEngine;
using UnityEngine.Pool;

namespace CursedOnion.Game.Miscellaneous
{
    [System.Serializable]
    public class ParticleEntry
    {
        public string ParticleEffectName;
        public GameObject ParticlePrefab;
        ParticleManager particleManager;
        
        public void SetManager(ParticleManager particleManager)
        {
            this.particleManager = particleManager;
        }
        public GameObject CreateParticleInstance()
        {
            var go = Object.Instantiate(ParticlePrefab);
            go.GetComponent<ParticleObject>().Initialize(ParticleEffectName, particleManager);
            return go;
        }
    }
    
    [CreateAssetMenu(fileName = "ParticleManager", menuName = "Game/Particles/Particle Manager")]
    public class ParticleManager : ScriptableObject
    {
        [SerializeField] private List<ParticleEntry> particleEntries;
        private Dictionary<string, ObjectPool<GameObject>> particlePools;
        
        public void Initialize()
        {
            particlePools = new Dictionary<string, ObjectPool<GameObject>>();
            foreach (var entry in particleEntries)
            {
                entry.SetManager(this);
                if (!particlePools.ContainsKey(entry.ParticleEffectName))
                {
                    particlePools.Add(entry.ParticleEffectName, PoolHelper.CreatePool(entry.CreateParticleInstance));
                }
            }
        }
        
        public void SpawnParticleAt(string particleEffectName, Vector3 position)
        {
            if (!particlePools.ContainsKey(particleEffectName)) return;
            
            var gameObject = particlePools[particleEffectName].Get();
            if (gameObject == null || gameObject == false)
            {
                gameObject = particleEntries.First(entry => entry.ParticleEffectName == particleEffectName).CreateParticleInstance();
            }
            gameObject.transform.position = position;
            gameObject.GetComponent<ParticleObject>().Play();
        }
        public void ReturnParticle(string particleEffectName, GameObject particle)
        {
            if (!particlePools.ContainsKey(particleEffectName)) return;
            
            particlePools[particleEffectName]?.Release(particle);
        }
    }
}