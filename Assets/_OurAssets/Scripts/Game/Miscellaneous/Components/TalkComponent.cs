using System;
using System.Collections;
using CursedOnion.Extensions;
using CursedOnion.Locators;
using Reflex.Attributes;
using Reflex.Core;
using UnityEngine;

namespace CursedOnion.Game.Miscellaneous
{
    
    public class TalkComponent : MonoBehaviour
    {
        [Inject] TextParticleManager textParticleManager;
        [Inject] RuntimeVariableLocator locator;

        private TalkData talkData;
        
        public void Initialize(TalkData data)
        {
            gameObject.InjectDependencies();
            
            talkData = data;
            
            if (talkData != null)
                StartCoroutine(RandomTalkRoutine());
        }
        void OnDestroy() => StopAllCoroutines();
        
        public void Talk(string text)
        {
            var spawnPos = transform.position - locator.GlobalCamera.GetForward() * 0.2f;
            textParticleManager?.SpawnTextAt(text, spawnPos);
        }
        public void TalkWithKey(string key)
        {
            var spawnPos = transform.position - locator.GlobalCamera.GetForward() * 0.2f;
            textParticleManager?.SpawnKeyTextAt(key, spawnPos);
        }
        private IEnumerator RandomTalkRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(talkData.GetNewRandomInterval());

                if (!talkData.TryGetWeightedRandomTalkKey(out string key) || gameObject.IsNull(this)) continue;

                TalkWithKey(key);
            }
        }
    }
}
