using System;
using CursedOnion.Game.Commands;
using CursedOnion.Game.Events;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Modes.Level.BattleEditor.UI;
using CursedOnion.Game.Systems.Level;
using Reflex.Attributes;
using TMPro;
using UnityEngine;

namespace CursedOnion.Game.General.UI.Canvases.Level
{
    public class UnitInformationWindow : MonoBehaviour
    {
        [SerializeField] StatDataInspector inspector;
        [Inject] LevelEvents levelEvents;
        
        void Awake()
        {
            ClearInspector();
        }
        private void OnEnable()
        {
            levelEvents.OnStatDataSelected += UpdateStatsDisplay;
            levelEvents.OnEntitySelected += UpdateDisplayWithEntity;
            levelEvents.OnNoEntitySelected += ClearInspector;
        }
        private void OnDisable()
        {
            levelEvents.OnStatDataSelected -= UpdateStatsDisplay;
            levelEvents.OnEntitySelected -= UpdateDisplayWithEntity;
            levelEvents.OnNoEntitySelected -= ClearInspector;
        }

        void UpdateDisplayWithEntity(SimpleEntity entity)
        {
            UpdateStatText(entity);
        }
        void UpdateStatsDisplay(StatData statData)
        {
            Debug.Log(statData);
            
            if(statData == null) ClearInspector();
            else UpdateDataText(statData);
        }
        void ClearInspector()
        {
            inspector.ClearInspector();
        }
        void UpdateDataText(StatData data)
        {
            inspector.SetInspectorStatData(data);
        }

        void UpdateStatText(SimpleEntity entity)
        {
            inspector.SetInspectorStats(entity);
        }
    }
}