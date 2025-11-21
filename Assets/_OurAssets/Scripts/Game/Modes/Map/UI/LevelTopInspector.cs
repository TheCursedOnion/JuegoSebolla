using System;
using CursedOnion.Game.Events;
using CursedOnion.Game.General.UI.Buttons;
using CursedOnion.Game.Localization;
using CursedOnion.Game.Objects;
using CursedOnion.Locators;
using Reflex.Attributes;
using UltEvents;
using UnityEngine;

namespace CursedOnion.Game.Modes.Map.UI
{
    public class LevelTopInspector : MonoBehaviour
    {
        [Inject] MapEvents mapEvents;
        [Inject] MapManager mapManager;
        [Inject] RuntimeVariableLocator variableLocator;
        
        [SerializeField] UIButton previousButton;
        [SerializeField] UIButton nextButton;
        [SerializeField] LocalizedGUIText levelName;
        
        LevelInformation currentLevelInformation;
        [SerializeField] UltEvent<string> OnLevelSelected;
        
        private void OnEnable()
        {
            mapEvents.OnLevelSelected += ProcessLevelSelected;
        }
        private void OnDisable()
        {
            mapEvents.OnLevelSelected -= ProcessLevelSelected;
        }
        void ProcessLevelSelected(LevelInformation levelInformation)
        {
            if(currentLevelInformation == levelInformation) return;
            
            currentLevelInformation = levelInformation;
            var levelType = levelInformation.LevelEnumType;
            previousButton.gameObject.SetActive(levelType != LevelInformation.LevelType.Start);
            nextButton.gameObject.SetActive(levelType != LevelInformation.LevelType.End);
            
            levelName.SetKey(levelInformation.NameKey);
        }

        public void CallNextLevel()
        {
            mapManager.MoveToNextLevel();
        }
        public void CallPreviousLevel()
        {
            mapManager.MoveToPreviousLevel();
        }

        public void PlayLevel()
        {
            if(currentLevelInformation == null) return;
            
            Debug.Log($"Playing level {currentLevelInformation.levelScene}");
            variableLocator.LastLevelPlayed = currentLevelInformation.LevelIndex;
            OnLevelSelected?.Invoke(currentLevelInformation.levelScene);
        }
    }
}