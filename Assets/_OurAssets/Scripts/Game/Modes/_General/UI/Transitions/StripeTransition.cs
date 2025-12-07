using System.Collections;
using System.Threading.Tasks;
using CursedOnion.Locators;
using NaughtyAttributes;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace CursedOnion.Game.Modes.General.UI.Transitions
{
    public class StripeTransition : UITransition
    {
        [Inject] UITransitionLocator transitionLocator;
        [SerializeField, MinMaxSlider(-3, 3f)] private Vector2 alphaStripeRange;
        protected override void Awake()
        {
            base.Awake();
            transitionLocator.AddTransition(TransitionType.Stripe ,this);
        }
        
        private static readonly int AlphaStripe = Shader.PropertyToID("_AlphaStripe");
        private static readonly int ReverseRotation = Shader.PropertyToID("_ReverseRotation");
        protected override IEnumerator HandleFullTransition(float duration)
        {
            float halfDuration = duration * 0.5f;
            Image.color = TransitionData.Color;
            
            yield return AnimateStripe(halfDuration, 0f, alphaStripeRange.x, alphaStripeRange.y);

            if (TransitionData.MidPointAction != null)
            {
                var task = TransitionData.MidPointAction.Invoke();
                yield return new WaitUntil(()=> task.IsCompleted);
            }

            yield return new WaitForSecondsRealtime(TransitionData.InBetweenTime);
            
            yield return AnimateStripe(halfDuration, 1f, alphaStripeRange.y, alphaStripeRange.x);
            TransitionData.EndPointAction?.Invoke();
            DoingTransition = false;
        }
        
        protected override IEnumerator HandleHalfTransition(float duration, bool isOpening)
        {
            if (isOpening)
            {
                Image.color = TransitionData.Color;
                yield return AnimateStripe(duration, 0f, alphaStripeRange.x, alphaStripeRange.y);
                
                if (TransitionData.MidPointAction != null)
                {
                    var task = TransitionData.MidPointAction.Invoke();
                    yield return new WaitUntil(()=> task.IsCompleted);
                }
            }
            else
            {
                yield return new WaitForSecondsRealtime(TransitionData.InBetweenTime);
                yield return AnimateStripe(duration, 1f, alphaStripeRange.y, alphaStripeRange.x);
                TransitionData.EndPointAction?.Invoke();
                DoingTransition = false;
            }
        }
        private IEnumerator AnimateStripe(float duration, float reverseRotationValue, float fromAlpha, float toAlpha)
        {
            MaterialInstance.SetFloat(ReverseRotation, reverseRotationValue);

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                float alpha = Mathf.Lerp(fromAlpha, toAlpha, t);
                MaterialInstance.SetFloat(AlphaStripe, alpha);

                yield return null;
            }
        }

        protected override void ResetProperties()
        {
            Image.color =  TransitionData.Color = Color.black;
            TransitionData.InBetweenTime = 0.2f;
            TransitionData.MidPointAction = null;
            TransitionData.EndPointAction = null;
            
            MaterialInstance.SetFloat(ReverseRotation, 0);
        }
    }
}
