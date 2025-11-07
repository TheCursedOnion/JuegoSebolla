using System;
using System.Linq;
using CursedOnion.Game.Commands;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Events;
using CursedOnion.Game.Systems.Level;
using Reflex.Attributes;
using TMPro;
using UnityEngine;

namespace CursedOnion.Game.Modes.Level.BattleEditor.UI
{
    public class UnitEditorSpawner : MonoBehaviour
    {
        [Inject] LevelManager levelManager;
        LevelEvents levelEvents;
        
        GameObject selectedUnit;
        private CommandParameters spawnParameters;
        private CommandParameters eraseParameters;

        private void Awake()
        {
            levelEvents = levelManager.LevelEvents;
            
            CommandParameters.Builder builder = new CommandParameters.Builder();
            builder.SetExecuteOnce(false).SetLevelManager(levelManager);
            
            spawnParameters = builder.Build();
            eraseParameters = builder.Build();
            
        }

        private void OnEnable()
        {
            levelEvents.OnNoEntitySelected += UnselectUnit;
        }
        private void OnDisable()
        {
            levelEvents.OnNoEntitySelected -= UnselectUnit;
        }

        public void ToggleSelectForSpawn(GameObject unitPrefab)
        {
            if (selectedUnit != null && selectedUnit == unitPrefab)
            {
                levelEvents.SelectEntity(null);
                levelEvents.CancelPreparedCommand();
                return;
            }
            
            selectedUnit = unitPrefab;
            spawnParameters.EntityPrefab = selectedUnit;
            
            Unit unit = unitPrefab.GetComponent<Unit>();
            levelEvents.SelectEntity(unit);

            levelEvents.CallPrepareCommand<SpawnCommand>(spawnParameters);
        }
        void UnselectUnit()
        {
            selectedUnit = null;
        }
        
        public void ToggleEraser()
        {
            if (selectedUnit != null)
            {
                levelEvents.SelectEntity(null);
                levelEvents.CancelPreparedCommand();
            }

            levelEvents.CallPrepareCommand<EraseCommand>(eraseParameters);
        }

        public void StartBattle()
        {
            levelManager.SetNewLevelState(LevelState.InBattle);
        }

        
    }
}
