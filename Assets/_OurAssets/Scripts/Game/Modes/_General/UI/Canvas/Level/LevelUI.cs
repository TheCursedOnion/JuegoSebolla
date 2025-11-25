
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
        [Inject] LevelManager levelManager;
        
        [SerializeField] private UnitActionsWindow actionsWindow;
        
        [SerializeField, BoxGroup(CameraContainer)] private GameObject cameraButtonsContainer;
        [SerializeField, BoxGroup(SettingsContainer)] private GameObject settingsContainer;
        
        [SerializeField, BoxGroup(GameplayContainer)] private GameObject gameplayContainer;
        [SerializeField, BoxGroup(GameplayContainer)] private GameObject battleEditorScreen;
        [SerializeField, BoxGroup(GameplayContainer)] private GameObject battleScreen;

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

        #region Settings Region
        public void Pause()
        {
            settingsContainer.SetActive(true);
            cameraButtonsContainer.SetActive(false);
            gameplayContainer.SetActive(false);
        }

        public void Unpause()
        {
            settingsContainer.SetActive(false);
            cameraButtonsContainer.SetActive(true);
            gameplayContainer.SetActive(true);
        }
        #endregion
        
        #region Gameplay Region
        void OnChangeLevelState(LevelState previousState, LevelState newState)
        {
            switch (newState)
            {
                case LevelState.InDialog: break;
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