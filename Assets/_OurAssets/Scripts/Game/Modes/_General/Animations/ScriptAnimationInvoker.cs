using CursedOnion.Game.Logic.Services;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Modes.General.Animations
{
    public class ScriptAnimationInvoker : MonoBehaviour
    {
        [Inject] ScriptAnimationService scriptAnimationService;
        [SerializeField] private AnimationParameters animationParameters;

        public void InvokeAnimation()
        {
            scriptAnimationService.InvokeAnimation(animationParameters);
        }
    }
}