using Fungus;
using UnityEngine;

namespace CursedOnion.Game.Dialog
{
    [CommandInfo("Custom", "Play Anim State Delayed", "Detiene el flujo hasta que el jugador haga click o toque la pantalla")]
    public class PlayAnimStateDelayed : Command
    {
        public Animator animator;
        public string animationName;
        public float delay = 0.5f;
        public override void OnEnter()
        {
            StartCoroutine(ChangeAnimState());
            Continue();
        }
        
        private System.Collections.IEnumerator ChangeAnimState()
        {
            yield return new WaitForSeconds(delay);
            animator.Play(animationName, -1, 0);
        }
    }
    
    [CommandInfo("Custom", "WaitForInput", "Detiene el flujo hasta que el jugador haga click o toque la pantalla")]
    public class WaitForInput : Command
    {
        public float securityDelay = 0.5f;
        private bool waiting = true;
        private bool canDetect = false;
        
        public override void OnEnter()
        {
            waiting = true;
            canDetect = false;
            StartCoroutine(EnableDetectionAfterDelay());
        }
        
        private System.Collections.IEnumerator EnableDetectionAfterDelay()
        {
            yield return new WaitForSeconds(securityDelay);
            canDetect = true;
        }
        void Update()
        {
            if (!waiting || !canDetect) return;

            // Ratón
            if (Input.GetMouseButtonDown(0))
            {
                waiting = false;
                Continue();
            }

            // Táctil
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                waiting = false;
                Continue();
            }
        }

        
    }
}