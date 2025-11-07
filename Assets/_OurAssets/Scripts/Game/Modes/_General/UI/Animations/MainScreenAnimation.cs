using System.Collections;
using CursedOnion.Game.Modes.General.UI.Transitions;
using CursedOnion.Locators;
using NaughtyAttributes;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace CursedOnion.Game.Modes.General.UI.Animations
{
    public class MainScreenAnimation : UIAnimation
    {
        [Inject] UITransitionLocator transitionLocator;
        
        [SerializeField] private Gradient gradient;
        [SerializeField] private AnimationCurve curvatureRange;
        [SerializeField] RawImage rawImage;
        Material materialInstance;
        
        private static readonly int FadeColor = Shader.PropertyToID("_FadeColor");
        private static readonly int Curvature = Shader.PropertyToID("_Curvature");
        protected override void Awake()
        {
            rawImage = GetComponent<RawImage>();
            materialInstance = Instantiate(rawImage.material);
            rawImage.material = materialInstance;
        }
        
        public override void DoAnimation(UIAnimationData animationData)
        {  
            if(ActiveAnimation != null) StopCoroutine(ActiveAnimation);

            ActiveAnimation = StartCoroutine(HandleAnimation(animationData));
        }
        IEnumerator HandleAnimation(UIAnimationData animationData)
        {
            OnAnimationStart?.Invoke();
            yield return new WaitForSeconds(animationData.StartDelay);
            OnAnimationDelayedStart?.Invoke();
            
            yield return AnimateScreen(animationData.Duration);
            OnAnimationEnd?.Invoke();
            
            yield return new WaitForSeconds(animationData.EndDelay);
            OnAnimationDelayedEnd?.Invoke();
            
            ActiveAnimation = null;
        }
        private IEnumerator AnimateScreen(float duration)
        {
            materialInstance.SetColor(FadeColor, gradient.Evaluate(0));

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                
                Color color = gradient.Evaluate(t);
                materialInstance.SetColor(FadeColor, color);
                
                float curvature = curvatureRange.Evaluate(t);
                materialInstance.SetFloat(Curvature, curvature);

                yield return null;
            }
            
            materialInstance.SetColor(FadeColor,  gradient.Evaluate(1));
        }
        public override void ResetAnimation()
        {
            if(ActiveAnimation != null) StopCoroutine(ActiveAnimation);
            materialInstance.SetColor(FadeColor, gradient.Evaluate(0));
            materialInstance.SetFloat(Curvature, curvatureRange.Evaluate(0));
        }
    }
}