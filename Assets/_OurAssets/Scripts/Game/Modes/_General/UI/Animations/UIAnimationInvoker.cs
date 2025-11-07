using UnityEngine;

namespace CursedOnion.Game.Modes.General.UI.Animations
{
    public class UIAnimationInvoker : MonoBehaviour
    {
        [SerializeField] private UIAnimationData animationData;

        public void InvokeAnimation(UIAnimation animation)
        {
            animation.DoAnimation(animationData);
        }
    }
}