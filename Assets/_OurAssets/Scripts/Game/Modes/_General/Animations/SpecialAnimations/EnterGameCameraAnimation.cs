using CursedOnion.Game.Cameras;
using UnityEngine;

namespace CursedOnion.Game.Modes.General.Animations
{
    public class EnterGameCameraAnimation : ScriptAnimation
    {
        [SerializeField] CinemachineContainer cinemachineContainer;

        [SerializeField] private float targetTiltValue;
        [SerializeField] private Vector3 targetOffset;
        [SerializeField] private float offsetAdjustTime;

        public override void DoAnimation(AnimationParameters animationParameters)
        {
            cinemachineContainer.SetTiltCenter(targetTiltValue);
            cinemachineContainer.SetOffset(targetOffset, offsetAdjustTime);
        }
    }
}