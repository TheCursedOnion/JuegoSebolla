using System;
using CursedOnion.Game.Commands;
using CursedOnion.Game.Events;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Systems.Level;
using Reflex.Attributes;
using TMPro;
using UnityEngine;

namespace CursedOnion.Game.General.UI.Canvases.Level
{
    public class UnitInformationWindow : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI statsText;
        [Inject] LevelEvents levelEvents;
        private void OnEnable()
        {
            levelEvents.OnStatDataSelected += UpdateStatsDisplay;
            levelEvents.OnEntitySelected += UpdateDisplayWithEntity;
            levelEvents.OnNoEntitySelected += ClearTextDisplay;
        }
        private void OnDisable()
        {
            levelEvents.OnStatDataSelected -= UpdateStatsDisplay;
            levelEvents.OnEntitySelected -= UpdateDisplayWithEntity;
            levelEvents.OnNoEntitySelected -= ClearTextDisplay;
        }

        void UpdateDisplayWithEntity(SimpleEntity entity)
        {
            UpdateStatText(entity);
        }
        void UpdateStatsDisplay(StatData statData)
        {
            Debug.Log(statData);
            
            if(statData == null) ClearTextDisplay();
            else UpdateDataText(statData);
        }
        void ClearTextDisplay()
        {
            if (statsText == null) return;
            statsText.text = "";
        }
        void UpdateDataText(StatData data)
        {
            if (statsText == null) return;
            
            statsText.text = $"{data.GetName()} -> LOS DATOS";
        }

        void UpdateStatText(SimpleEntity entity)
        {
            var stats = entity.GetStats();
            statsText.text = $"{entity.StatData.GetName()} -> {stats.InitiativeStat}\n" +
                             $"HP -> {stats.CurrentHealthStat}\n" +
                             $"MaxHP -> {stats.MaxHealthStat}\n" +
                             $"attack -> {stats.AttackStat}\n" +
                             $"defense -> {stats.DefenseStat}\n" +
                             $"movement -> {stats.MovementStat}\n" +
                             $"price -> {stats.PriceStat}";
        }
    }
}