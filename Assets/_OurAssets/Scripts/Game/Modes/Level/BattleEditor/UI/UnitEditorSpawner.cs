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
using UnityEngine.UI;

namespace CursedOnion.Game.Modes.Level.BattleEditor.UI
{
    public class UnitEditorSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject unitPrefab;
        [SerializeField] private Scrollbar scrollbar;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private HorizontalLayoutGroup horizontalLayoutGroup;
        
        [Inject] UIEvents uiEvents;
        [Inject] LevelManager levelManager;
        [Inject] LevelEvents levelEvents;
        
        GameObject selectedUnit;
        bool wasScrollbarHidden = false;
        private int lastMode = -1;
        StatData lastSelectedStats;
        
        private CommandParameters spawnParameters;
        private CommandParameters eraseParameters;
        
        private void Awake()
        {
            AttributeInjector.Inject(this, gameObject.scene.GetSceneContainer());
            
            CommandParameters.Builder builder = new CommandParameters.Builder();
            builder.SetExecuteOnce(false).SetLevelManager(levelManager);
            
            spawnParameters = builder.Build();
            eraseParameters = builder.Build();
            
            wasScrollbarHidden = !scrollbar.gameObject.activeSelf;
            scrollRect.horizontalScrollbar.value = 0;
        }

        void OnEnable()
        {
            UpdatePadding(wasScrollbarHidden);
            scrollRect.horizontalScrollbar.value = 0;
        }
        
        private void Update()
        {
            bool isScrollbarHidden = !scrollbar.gameObject.activeSelf;
            if (isScrollbarHidden != wasScrollbarHidden)
            {
                wasScrollbarHidden = isScrollbarHidden;
                UpdatePadding(isScrollbarHidden);
            }
        }
        private void UpdatePadding(bool scrollbarHidden)
        {
            var p = horizontalLayoutGroup.padding;
            if (scrollbarHidden)
            {
                p.bottom = 50;
            }
            else
            {
                p.bottom = 15;
            }

            horizontalLayoutGroup.padding = p;
            horizontalLayoutGroup.SetLayoutHorizontal();
            horizontalLayoutGroup.SetLayoutVertical();
        }
        private void OnDisable()
        {
            uiEvents.UnselectAllButtons();
            DeselectAll();
            EnableEditorVisualEffect(false);
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
                levelEvents.CancelPreparedCommand();
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
