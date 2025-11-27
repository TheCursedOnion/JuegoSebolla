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

        private int lastMode = -1;
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
                EnableEditorVisualEffect(lastMode != 1);
                return;
            }
            
            lastMode = 1;
            lastSelectedStats = statData;
            
            selectedUnit = unitPrefab;
            spawnParameters.Prefab = unitPrefab;
            spawnParameters.EntityStatData = statData;
                
            levelEvents.SelectStatData(statData);
            levelEvents.EnableBlackAndWhite(true);
            levelEvents.CallPrepareCommand<SpawnCommand>(spawnParameters);
        }
        public void ToggleEraser()
        {
            if (lastSelectedStats != null)
            {
                DeselectAll();
            }

            if (lastMode == 0)
            {
                EnableEditorVisualEffect(false);
                return;
            }
            

            lastMode = 0;
            levelEvents.EnableBlackAndWhite(true);
            levelEvents.CallPrepareCommand<EraseCommand>(eraseParameters);
        }

        public void DeselectAll()
        {
            lastSelectedStats = null;
            levelEvents.SelectStatData(null);
            levelEvents.CancelPreparedCommand();
        }
        public void EnableEditorVisualEffect(bool enable)
        {
            levelEvents.EnableBlackAndWhite(enable);
            lastMode = -1;
        }

        public void StartBattle()
        {
            levelManager.TrySetNewState(LevelState.InBattle);
        }
    }
}
