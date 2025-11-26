using NaughtyAttributes;
using UnityEngine;

namespace CursedOnion.Game.Modes.General.Animations
{
    public class AnimationListener : MonoBehaviour
    {
        [SerializeField, ReadOnly] AnimatorController animatorController;

        public void SetController(AnimatorController controller)
        {
            animatorController = controller;
        }
        
        public void InvokeAnimationStart(string animationName)
        {
            animatorController.ProcessStartedAnimation(animationName);
        }
        
        public void InvokeAnimationEvent(string eventName)
        {
            animatorController.ProcessAnimationEvent(eventName);
        }
        
        public void InvokeAnimationEnd(string animationName)
        {
            animatorController.ProcessFinishedAnimation(animationName);
        }
    }
}
