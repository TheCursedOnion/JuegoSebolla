using UltEvents;
using UnityEngine;

namespace CursedOnion.Game.Modes.General.Animations
{
    public interface IScriptAnimation
    {
        public void DoAnimation(AnimationParameters animationParameters);
    }
    
    public interface IResetableAnimation
    {
        public void ResetAnimation();
    }
    
    public abstract class ScriptAnimation : MonoBehaviour, IScriptAnimation
    {
        [SerializeField] protected UltEvent OnAnimationStart;
        [SerializeField] protected UltEvent OnAnimationDelayedStart;
        [SerializeField] protected UltEvent OnAnimationEnd;
        [SerializeField] protected UltEvent OnAnimationDelayedEnd;

        protected Coroutine ActiveAnimation;
        public abstract void DoAnimation(AnimationParameters animationParameters);
    }
}