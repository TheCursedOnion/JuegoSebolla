using System;
using UnityEngine;

namespace CursedOnion.Game.Modes.General.Animations
{
    public class EntityAnimatorController : MonoBehaviour
    {
        public Animator animator;
        [SerializeField] private string testAnimationName;

        private Action onAnimationFinished;

        public void PlayAnimation(string animationName, Action onFinished = null)
        {
            if (animator == null) return;

            onAnimationFinished = onFinished;

            if (animationName.ToLower().Contains("heal")) //Para curar hay que reproducir la otra animaci�n antes
            {
                animator.SetBool("isHealing", true);
                animator.Play("buff", 0, 0f);
                return;
            }

            animator.SetBool("isHealing", false);
            animator.Play(animationName, 0);
        }

        public void OnAnimationEventFinished()
        {
            onAnimationFinished?.Invoke();
            onAnimationFinished = null;
        }

        public void TestPlayAnimation()
        {
            PlayAnimation(testAnimationName);
        }

        private void Reset()
        {
            if (animator == null)
                animator = GetComponent<Animator>();
        }
        
        
    }
}
