using System;
using CursedOnion.Game.Logic.Services.Pause;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion
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
    }
}
