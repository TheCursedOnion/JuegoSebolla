using System.Collections;
using System.Threading.Tasks;
using CursedOnion.Locators;
using NaughtyAttributes;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Modes.General.UI.Transitions
{
    public class FadeTransition : UITransition
    {
        [Inject] UITransitionLocator transitionLocator;
        [SerializeField, MinMaxSlider(0, 1f)] private Vector2 alphaRange;
        
        private static readonly int FadeColor = Shader.PropertyToID("_Color");
        private static readonly int Alpha = Shader.PropertyToID("_Alpha");
        protected override void Awake()
        {
            base.Awake();
            transitionLocator.AddTransition(TransitionType.Fade ,this);
        }
        protected override IEnumerator HandleFullTransition(float duration)
        {
            float halfDuration = duration * 0.5f;
            MaterialInstance.SetColor(FadeColor, TransitionData.Color);
            
            yield return AnimateFade(halfDuration, alphaRange.x, alphaRange.y);
            if (TransitionData.MidPointAction != null)
            {
                var task = TransitionData.MidPointAction.Invoke();
                yield return new WaitUntil(()=> task.IsCompleted);
            }
            
            yield return new WaitForSecondsRealtime(TransitionData.InBetweenTime);
            
            yield return AnimateFade(halfDuration, alphaRange.y, alphaRange.x);
            TransitionData.EndPointAction?.Invoke();
            DoingTransition = false;
        }
        
        protected override IEnumerator HandleHalfTransition(float duration, bool isOpening)
        {
            if (isOpening)
            {
                MaterialInstance.SetColor(FadeColor, TransitionData.Color);
                yield return AnimateFade(duration, alphaRange.x, alphaRange.y);
                
                if (TransitionData.MidPointAction != null)
                {
                    var task = TransitionData.MidPointAction.Invoke();
                    yield return new WaitUntil(()=> task.IsCompleted);
                }
            }
            else
            {
                yield return new WaitForSecondsRealtime(TransitionData.InBetweenTime);
                yield return AnimateFade(duration, alphaRange.y, alphaRange.x);
                Debug.Log("Transition ended");
                TransitionData.EndPointAction?.Invoke();
                DoingTransition = false;
            }
        }
        private IEnumerator AnimateFade(float duration, float fromAlpha, float toAlpha)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                float alpha = Mathf.Lerp(fromAlpha, toAlpha, t);
                MaterialInstance.SetFloat(Alpha, alpha);

                yield return null;
            }
        }

        protected override void ResetProperties()
        {
            TransitionData.InBetweenTime = 0.2f;
            TransitionData.MidPointAction = null;
            TransitionData.EndPointAction = null;
            
            MaterialInstance.SetFloat(Alpha, 0);
            MaterialInstance.SetColor(FadeColor, Color.black);
        }
    }
}