using CursedOnion.Game.Logic.Services;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Modes.General.Animations
{
    public class ScriptAnimationListener : MonoBehaviour
    {
        [SerializeField] private string animationTag;
        
        [Inject] ScriptAnimationService scriptAnimationService;
        private IScriptAnimation scriptAnimation;
        private void Awake()
        {
            scriptAnimation ??= GetComponent<IScriptAnimation>();
        }
        private void OnEnable()
        {
            scriptAnimationService.OnAnimationCall += CheckAnimationCall;
        }
        private void OnDisable()
        {
            scriptAnimationService.OnAnimationCall -= CheckAnimationCall;
        }

        void CheckAnimationCall(AnimationParameters animationParameters)
        {
            if(string.Equals(animationTag, animationParameters.AnimationTag))
                scriptAnimation.DoAnimation(animationParameters);
        }
        
    }
}