using System;
using System.Collections.Generic;
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
            levelEvents.OnStatDataSelected += UpdateDataText;
            levelEvents.OnEntitySelected += UpdateStatText;
            levelEvents.OnNoEntitySelected += ClearInspector;
        }
        private void OnDisable()
        {
            levelEvents.OnStatDataSelected -= UpdateDataText;
            levelEvents.OnEntitySelected -= UpdateStatText;
            levelEvents.OnNoEntitySelected -= ClearInspector;
        }
        void ClearInspector()
        {
            inspector.ClearInspector();
        }
        void UpdateDataText(StatData data)
        {
            if(data == null) ClearInspector();
            else inspector.SetInspectorStatData(data);
        }

        void UpdateStatText(SimpleEntity entity)
        {
            inspector.SetInspectorStats(entity);
        }
    }
}