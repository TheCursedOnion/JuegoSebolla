using UltEvents;
using UnityEngine;

namespace CursedOnion.Game.Modes.General.UI.Animations
{
    public abstract class UIAnimation : MonoBehaviour
    {
        [SerializeField] protected UltEvent OnAnimationStart;
        [SerializeField] protected UltEvent OnAnimationDelayedStart;
        [SerializeField] protected UltEvent OnAnimationEnd;
        [SerializeField] protected UltEvent OnAnimationDelayedEnd;

        protected Coroutine ActiveAnimation;
        
        protected abstract void Awake();
        public abstract void DoAnimation(UIAnimationData animationData);
        public abstract void ResetAnimation();
    }
}