using System;
using CursedOnion.Game.Logic.Services.Pause;
using CursedOnion.Game.Logic.Services;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Logic.Services
{
    public class PauseComponent : MonoBehaviour
    {
        [Inject] PauseService pauseService;
        
        [SerializeField] PauseLevel pauseWithLevel;
        [SerializeField] IPausable pausableObject;
        private bool isPaused = false;

        private void Awake()
        {
            pausableObject ??= GetComponent<IPausable>();
        }
        private void OnEnable()
        {
            pauseService.OnPauseUpdate += CheckPause;
        }
        private void OnDisable()
        {
            pauseService.OnPauseUpdate -= CheckPause;
        }

        private void CheckPause(PauseLevel updatedPauseLevel)
        {
            if(pausableObject == null) return;
            int pauseLevelToCheck = (int)updatedPauseLevel;

            if (!isPaused && pauseLevelToCheck >= (int)pauseWithLevel)
            {
                isPaused = true;
                pausableObject.Pause();
            }
            else if(isPaused && pauseLevelToCheck < (int)pauseWithLevel)
            {
                isPaused = false;
                pausableObject.Unpause();
            }
            
        }
        
        public void InvokePauseWithLevel(PauseLevel pauseLevel)
        {
            pauseService.Pause(pauseLevel);
        }
        public void InvokeUnpauseWithLevel(PauseLevel pauseLevel)
        {
            pauseService.Unpause(pauseLevel);
        }
    }
}
