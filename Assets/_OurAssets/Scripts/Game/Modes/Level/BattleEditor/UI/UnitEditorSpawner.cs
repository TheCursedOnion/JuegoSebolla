using System;
using System.Linq;
using CursedOnion.Game.Commands;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Events;
using CursedOnion.Game.Modes.General.UI.Events;
using CursedOnion.Game.Systems.Level;
using Reflex.Attributes;
using Reflex.Core;
using Reflex.Extensions;
using Reflex.Injectors;
using TMPro;
using UnityEngine;

namespace CursedOnion.Game.Modes.Level.BattleEditor.UI
{
    public class UnitEditorSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject unitPrefab;
        
        [Inject] UIEvents uiEvents;
        [Inject] LevelManager levelManager;
        LevelEvents levelEvents;
        
        GameObject selectedUnit;
        StatData lastSelectedStats;
        
        private CommandParameters spawnParameters;
        private CommandParameters eraseParameters;

        private void Awake()
        {
            AttributeInjector.Inject(this, gameObject.scene.GetSceneContainer());
            levelEvents = levelManager.LevelEvents;
            
            CommandParameters.Builder builder = new CommandParameters.Builder();
            builder.SetExecuteOnce(false).SetLevelManager(levelManager);
            
            spawnParameters = builder.Build();
            eraseParameters = builder.Build();
            
        }
        private void OnDisable()
        {
            uiEvents.UnselectAllButtons();
        }

        public void ToggleSelectForSpawn(UnitButtonSpawner buttonSpawner)
        {
            var statData = buttonSpawner.GetUnitStats();
            if (lastSelectedStats != null && lastSelectedStats == statData)
            {
                DeselectAll();
                return;
            }
            
            lastSelectedStats = statData;
            
            selectedUnit = unitPrefab;
            spawnParameters.Prefab = unitPrefab;
            spawnParameters.EntityStatData = statData;
                
            levelEvents.SelectStatData(statData);

            levelEvents.CallPrepareCommand<SpawnCommand>(spawnParameters);
        }
        public void ToggleEraser()
        {
            if (lastSelectedStats != null)
            {
                DeselectAll();
            }

            levelEvents.CallPrepareCommand<EraseCommand>(eraseParameters);
        }

        public void DeselectAll()
        {
            lastSelectedStats = null;
            levelEvents.SelectStatData(null);
            levelEvents.CancelPreparedCommand();
        }

        public void StartBattle()
        {
            levelManager.SetNewLevelState(LevelState.InBattle);
        }
    }
}
