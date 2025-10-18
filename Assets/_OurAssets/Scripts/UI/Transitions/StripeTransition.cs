using System.Collections;
using CursedOnion.Locators;
using NaughtyAttributes;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace CursedOnion.UI
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

            yield return AnimateStripe(halfDuration, 0f, alphaStripeRange.x, alphaStripeRange.y);
            MidPointAction?.Invoke();
            
            yield return new WaitForSecondsRealtime(InBetweenTime);
            
            yield return AnimateStripe(halfDuration, 1f, alphaStripeRange.y, alphaStripeRange.x);
            EndPointAction?.Invoke();
            DoingTransition = false;
        }
        
        protected override IEnumerator HandleHalfTransition(float duration, bool isOpening)
        {
            Debug.Log("Open: " + isOpening);
            if (isOpening)
            {
                yield return AnimateStripe(duration, 0f, alphaStripeRange.x, alphaStripeRange.y);
                MidPointAction?.Invoke();
            }
            else
            {
                yield return new WaitForSecondsRealtime(InBetweenTime);
                yield return AnimateStripe(duration, 1f, alphaStripeRange.y, alphaStripeRange.x);
                EndPointAction?.Invoke();
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
            MidPointAction = null;
            EndPointAction = null;
            Image.color = Color.black;
            InBetweenTime = 0.2f;
        }
    }
}
