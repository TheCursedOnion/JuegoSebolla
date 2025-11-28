using System;
using CursedOnion.Game.Localization;
using CursedOnion.Game.Systems.Level;
using CursedOnion.Locators;
using Reflex.Attributes;
using Reflex.Extensions;
using UnityEngine;

namespace CursedOnion
{
    public class LevelOutcomeController : MonoBehaviour
    {
        [Inject] LevelEvents levelEvents;
        [SerializeField] LocalizedGUIText outcomeText;
        [SerializeField] string victoryTextKey;
        [SerializeField] string defeatTextKey;
        
        public void Initialize()
        {
            levelEvents.OnLevelCompleted += ProcessLevelOutcome;
        }

        private void OnDestroy()
        {
            levelEvents.OnLevelCompleted -= ProcessLevelOutcome;
        }
        
        void ProcessLevelOutcome(bool hasWon)
        {
            string useKey = hasWon ? victoryTextKey : defeatTextKey;
            outcomeText.SetKey(useKey);
        }
    }
}
