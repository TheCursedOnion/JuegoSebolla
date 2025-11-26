using CursedOnion.Extensions;
using CursedOnion.Game.Logic.Services.Pause;
using CursedOnion.Game.Modes.Level.Battle.UI;
using CursedOnion.Game.Systems.Level;
using NaughtyAttributes;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.General.UI.Canvases.Level
{
    public class LevelUI : MonoBehaviour, IUICanvas, IPausable
    {
        const string SettingsContainer = "Settings Container Variables";
        const string GameplayContainer = "Gameplay Container Variables";
        const string CameraContainer = "Camera Container Variables";
        const string ResultsContainer = "Results Container Variables";
        [Inject] LevelManager levelManager;
        
        [SerializeField] private UnitActionsWindow actionsWindow;
        [SerializeField] float fadeTime = 0.5f;
        
        [SerializeField, BoxGroup(CameraContainer)] private CanvasGroup cameraButtonsGroup;
        
        [SerializeField, BoxGroup(GameplayContainer)] private CanvasGroup gameplayGroup;
        [SerializeField, BoxGroup(GameplayContainer)] private GameObject battleEditorScreen;
        [SerializeField, BoxGroup(GameplayContainer)] private GameObject battleScreen;
        
        [SerializeField, BoxGroup(SettingsContainer)] private CanvasGroup settingsGroup;
        
        [SerializeField, BoxGroup(ResultsContainer)] private CanvasGroup resultsGroup;

        private void OnEnable()
        {
            levelManager.LevelEvents.OnLevelStateChange += OnChangeLevelState;
            actionsWindow.Initialize(levelManager);
            OnChangeLevelState(LevelState.InBattle, levelManager.CurrentLevelState);
        }

        void OnDisable()
        {
            levelManager.LevelEvents.OnLevelStateChange -= OnChangeLevelState;
        }
        
        void DisableAllContainer()
        {
            settingsGroup.SetGroupActive(false, 0f);
            cameraButtonsGroup.SetGroupActive(false, 0f);
            gameplayGroup.SetGroupActive(false, 0f);
            resultsGroup.SetGroupActive(false, 0f);
        }
        void EnableOnlyContainer(CanvasGroup container)
        {
            DisableAllContainer();
            container.SetGroupActive(true, fadeTime);
        }
        void EnableOnlyContainers(params CanvasGroup[] container)
        {
            DisableAllContainer();
            foreach (var c in container) c.SetGroupActive(true, fadeTime);
        }

        #region Settings Region
        public void Pause(PauseLevel pauseLevel)
        {
            switch (pauseLevel)
            {
                case PauseLevel.Dialog: DisableAllContainer(); break;
                case PauseLevel.UI: EnableOnlyContainer(settingsGroup); break;
            }
        }

        public void Unpause()
        {
            EnableOnlyContainers(cameraButtonsGroup, gameplayGroup);
        }
        #endregion
        
        #region Gameplay Region
        void OnChangeLevelState(LevelState previousState, LevelState newState)
        {
            switch (newState)
            {
                case LevelState.InDialog:
                case LevelState.InBattleEditor: EnableScreen(battleEditorScreen); break;
                case LevelState.InBattle: EnableScreen(battleScreen); break;
                case LevelState.Finished: break;
            }
        }
        void DisableAllScreens()
        {
            battleEditorScreen.SetActive(false);
            battleScreen.SetActive(false);
        }
        void EnableScreen(GameObject screen)
        {
            DisableAllScreens();
            screen.SetActive(true);
        }
        #endregion

        #region Turn Region

        public void OnEndTurnButtonPressed()
        {
            var turnSystem = levelManager.GetTurnSystem();
            turnSystem.EndTurn();
        }

        public void OnStartTurnButtonPressed()
        {
            levelManager.GetTurnSystem().BeginBattle();
        }

        #endregion
        
    }
}