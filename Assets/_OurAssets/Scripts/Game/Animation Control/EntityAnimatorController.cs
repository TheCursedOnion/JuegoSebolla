using UnityEngine;

namespace CursedOnion
{
    public class EntityAnimatorController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private string testAnimationName;

        public void PlayAnimation(string animationName)
        {
            if (animator == null) return;

            if (animationName.ToLower().Contains("heal")) //Para curar hay que reproducir la otra animación antes
            {
                animator.SetBool("isHealing", true);
                animator.Play("base_buff");
                return;
            }

            animator.SetBool("isHealing", false);
            animator.Play(animationName);
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
