using System;
using System.Collections;
using CursedOnion.Extensions;
using CursedOnion.Locators;
using Reflex.Attributes;
using Reflex.Core;
using Unity.VisualScripting;
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

        public void Talk(string text, float delay)
        {
            StartCoroutine(TalkWithDelay(delay, () => Talk(text)));
        }
        public void TalkWithKey(string key)
        {
            var spawnPos = transform.position - locator.GlobalCamera.GetForward() * 0.2f;
            textParticleManager?.SpawnKeyTextAt(key, spawnPos);
        }
        public void TalkWithKey(string key, float delay)
        {
            StartCoroutine(TalkWithDelay(delay, () => TalkWithKey(key)));
        }

        private IEnumerator TalkWithDelay(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback?.Invoke();
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
