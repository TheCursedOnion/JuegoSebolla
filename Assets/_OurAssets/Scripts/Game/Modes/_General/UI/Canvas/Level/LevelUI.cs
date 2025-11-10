using System;
using System.Linq;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Events;
using CursedOnion.Game.Systems.Level;
using CursedOnion.ScriptableObjects;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.General.UI.Canvases.Level
{
    public class LevelUI : MonoBehaviour, IUICanvas
    {
        [Inject] LevelManager levelManager;
        [SerializeField] private UnitActionsWindow actionsWindow;

        [SerializeField] private GameObject battleEditorScreen;
        [SerializeField] private GameObject battleScreen;

        private void OnEnable()
        {
            levelManager.LevelEvents.OnLevelStateChange += OnChangeLevelState;
            OnChangeLevelState(LevelState.InBattle, levelManager.CurrentLevelState);
        }

        void OnDisable()
        {
            levelManager.LevelEvents.OnLevelStateChange -= OnChangeLevelState;
        }

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

        public void OnEndTurnButtonPressed()
        {
            var turnSystem = levelManager.GetTurnSystem();
            var activeUnits = turnSystem.GetActiveUnits();
            if (activeUnits == null || activeUnits.Count == 0)
            {
                Debug.Log("No hay unidades activas.");
                return;
            }

            foreach (var unit in activeUnits.ToList())
            {
                Debug.Log($"Terminando turno de {unit.name}");
                turnSystem.EndTurnForUnit(unit);
            }
        }

        public void OnStartButtonPressed()
        {
            levelManager.GetTurnSystem().StartRound();
        }
    }
}