using System;
using System.Collections.Generic;
using System.Linq;
using CursedOnion.GameLogic.Services.Pause;
using NaughtyAttributes;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Testing
{
    public class PauseTest : MonoBehaviour, IPausable
    {
        [Inject] PauseService _pauseService;
        [Inject] readonly IEnumerable<string> strings;
        void Start()
        {
            Debug.Log(string.Join(" ", strings));
        }
        
        [ReadOnly][SerializeField] int currentPauseLevel = 0;
        int pauseLevelCount = Enum.GetValues(typeof(PauseLevel)).Length;
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                currentPauseLevel = (currentPauseLevel+1) % pauseLevelCount;
                _pauseService.Pause((PauseLevel)currentPauseLevel);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _pauseService.UnpauseCurrentLevel();
            }
        }

        public void Pause()
        {
            Debug.Log($"{gameObject.name} pausing");
        }

        public void Unpause()
        {
            Debug.Log($"{gameObject.name} unpausing");
        }
    }
}
