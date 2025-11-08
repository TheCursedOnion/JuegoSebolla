using System;
using CursedOnion.Game.Modes.General.Animations;

namespace CursedOnion.Game.Logic.Services
{
    public class ScriptAnimationService : IService
    {
        public Action<AnimationParameters> OnAnimationCall;
        public void InvokeAnimation(AnimationParameters animationParameters)
        {
            OnAnimationCall?.Invoke(animationParameters);
        }
    }
}