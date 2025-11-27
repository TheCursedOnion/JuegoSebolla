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
        
        private void Awake()
        {
            mapEvents.OnLevelSelected += ProcessLevelSelected;
        }
        private void OnDestroy()
        {
            mapEvents.OnLevelSelected -= ProcessLevelSelected;
        }
        void ProcessLevelSelected(LevelInformation levelInformation)
        {
            if(currentLevelInformation == levelInformation) return;
            
            currentLevelInformation = levelInformation;
            
            LevelInformation.LevelType levelType = levelInformation.LevelEnumType;
            
            int currentLevelIndex = currentLevelInformation.LevelIndex;
            int completedLevels = variableLocator.LastCompletedLevel;

            bool isPreviousButtonEnabled = levelType != LevelInformation.LevelType.Start;
            previousButton.gameObject.SetActive(isPreviousButtonEnabled);
            
            bool isNextButtonEnabled = levelType != LevelInformation.LevelType.End && completedLevels >= currentLevelIndex;
            nextButton.gameObject.SetActive(isNextButtonEnabled);
            
            levelName.SetKey(levelInformation.BaseKey);
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
            variableLocator.LastPlayedLevel = currentLevelInformation.LevelIndex;
            OnLevelSelected?.Invoke(currentLevelInformation.levelScene);
        }
    }
}