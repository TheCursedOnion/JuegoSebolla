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
            
            if (talkData != null && talkData.HasRandomInterval)
                StartCoroutine(RandomTalkRoutine());
        }
        void OnDestroy() => StopAllCoroutines();
        
        public void Talk(string text, Color? color = null)
        {
            var spawnPos = transform.position - locator.GlobalCamera.GetForward() * 0.2f;
            textParticleManager?.SpawnTextAt(text, spawnPos, color);
        }

        public void RandomTalk()
        {
            if (talkData == null || !talkData.TryGetWeightedRandomTalkKey(out string key) || gameObject.IsNull(this)) return;
            if(string.IsNullOrEmpty(key)) return;
            TalkWithKey(key);
        }
        public void Talk(string text, float delay, Color? color = null)
        {
            StartCoroutine(TalkWithDelay(delay, () => Talk(text, color)));
        }
        public void TalkWithKey(string key, Color? color = null)
        {
            var spawnPos = transform.position - locator.GlobalCamera.GetForward() * 0.2f;
            textParticleManager?.SpawnKeyTextAt(key, spawnPos, color);
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

                RandomTalk();
            }
        }
    }
}
