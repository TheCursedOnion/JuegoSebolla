using CursedOnion.Extensions;
using CursedOnion.Game.Audio;
using CursedOnion.Game.Events;
using CursedOnion.Game.Logic.Services.Pause;
using CursedOnion.Game.Modes.General.UI.Events;
using CursedOnion.Game.Modes.Level.Battle.UI;
using CursedOnion.Game.Systems.Level;
using CursedOnion.Locators;
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
        [Inject] RuntimeVariableLocator variableLocator;
        
        [SerializeField] private float endFadeDelay = 1f;
        [SerializeField] float fadeTime = 0.5f;
        
        [SerializeField, BoxGroup(CameraContainer)] private CanvasGroup cameraButtonsGroup;
        
        [SerializeField, BoxGroup(GameplayContainer)] private CanvasGroup gameplayGroup;
        [SerializeField, BoxGroup(GameplayContainer)] private GameObject battleEditorScreen;
        [SerializeField, BoxGroup(GameplayContainer)] private GameObject battleScreen;
        
        [SerializeField, BoxGroup(GameplayContainer)] private UnitInformationWindow unitInformationWindow;
        [SerializeField, BoxGroup(GameplayContainer)] private UnitActionsWindow actionsWindow;
        [SerializeField, BoxGroup(GameplayContainer)] private TurnInspector turnInspector;
        
        [SerializeField, BoxGroup(SettingsContainer)] private CanvasGroup settingsGroup;
        
        [SerializeField, BoxGroup(ResultsContainer)] private CanvasGroup resultsGroup;
        
        bool hasDoneIntro = false;
        
        private void OnEnable()
        {
            levelManager.LevelEvents.OnLevelStateChange += OnChangeLevelState;
            levelManager.LevelEvents.OnIntroFinished += OnIntroDone;
            
            unitInformationWindow.Initialize(levelManager);
            actionsWindow.Initialize(levelManager);
            turnInspector.Initialize(levelManager);
            
            resultsGroup.GetOrAddComponent<LevelOutcomeController>().Initialize();
            
            DisableAllGroups();
            OnChangeLevelState(LevelState.InBattle, levelManager.CurrentLevelState);
        }

        void OnDisable()
        {
            levelManager.LevelEvents.OnIntroFinished -= OnIntroDone;
            levelManager.LevelEvents.OnLevelStateChange -= OnChangeLevelState;
        }
        
        void DisableAllGroups()
        {
            settingsGroup.SetGroupActive(false, 0f);
            cameraButtonsGroup.SetGroupActive(false, 0f);
            gameplayGroup.SetGroupActive(false, 0f);
            resultsGroup.SetGroupActive(false, 0f);
        }
        void EnableOnlyGroup(CanvasGroup container, float delay = 0f)
        {
            DisableAllGroups();
            container.SetGroupActive(true, fadeTime, delay);
        }
        void EnableOnlyGroups(params CanvasGroup[] container)
        {
            DisableAllGroups();
            foreach (var c in container) c.SetGroupActive(true, fadeTime);
        }

        void OnIntroDone()
        {
            hasDoneIntro = true;
            EnableOnlyGroups(cameraButtonsGroup, gameplayGroup);
        }

        #region Pause Region
        public void Pause(PauseLevel pauseLevel)
        {
            switch (pauseLevel)
            {
                case PauseLevel.Dialog: DisableAllGroups(); break;
                case PauseLevel.UI: EnableOnlyGroup(settingsGroup); break;
            }
        }

        public void Unpause()
        {
            if(hasDoneIntro)
                EnableOnlyGroups(cameraButtonsGroup, gameplayGroup);
        }
        #endregion
        
        #region Gameplay Region
        void OnChangeLevelState(LevelState previousState, LevelState newState)
        {
            switch (newState)
            {
                case LevelState.InDialog:
                case LevelState.InBattleEditor: EnableScreen(battleEditorScreen);
                    if (variableLocator !=  null && variableLocator.MusicPlayer)
                    {
                        switch (levelManager.LevelAsset.LevelData.TimePeriod)
                        {
                            case LevelTimePeriod.Greece: variableLocator.MusicPlayer.RequestMusic(MusicType.GreeceGameplay); break;
                            case LevelTimePeriod.Egypt: variableLocator.MusicPlayer.RequestMusic(MusicType.EgyptGameplay); break;
                            case LevelTimePeriod.Japan: variableLocator.MusicPlayer.RequestMusic(MusicType.JapanGameplay); break;
                        }
                    }
                    break;
                case LevelState.InBattle: EnableScreen(battleScreen); break;
                case LevelState.Finished: DisableAllGroups(); break;
                case LevelState.InResults: EnableOnlyGroup(resultsGroup, endFadeDelay); break;
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