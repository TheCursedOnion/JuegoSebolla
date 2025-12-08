using System;
using CursedOnion.Game.Systems.Level;
using Reflex.Attributes;
using TMPro;
using UnityEngine;

namespace CursedOnion.Game.Modes.Level.Battle.UI
{
    public class RoundCounter : MonoBehaviour
    {
        [Inject] LevelEvents levelEvents;
        [SerializeField] private TextMeshProUGUI roundCounterText;

        int roundCounter = 0;
        void Awake()
        {
            roundCounterText.text = roundCounter.ToString();
            levelEvents.OnRoundPassed += ProcessRoundPassed;
        }

        private void OnDestroy()
        {
            levelEvents.OnRoundPassed -= ProcessRoundPassed;
        }

        void ProcessRoundPassed()
        {
            roundCounter++;
            roundCounterText.text = roundCounter.ToString();
        }
    }
}