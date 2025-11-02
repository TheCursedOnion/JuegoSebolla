using System;
using CursedOnion.Game.Commands;
using CursedOnion.Game.Events;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Systems.Level;
using Reflex.Attributes;
using TMPro;
using UnityEngine;

namespace CursedOnion.UI.Canvases.Level
{
    public class UnitInformationWindow : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI statsText;
        [Inject] LevelEvents levelEvents;
        private void OnEnable()
        {
            levelEvents.OnEntitySelected += UpdateStatsDisplay;
            levelEvents.OnNoEntitySelected += ClearStatsDisplay;
        }
        private void OnDisable()
        {
            levelEvents.OnEntitySelected -= UpdateStatsDisplay;
            levelEvents.OnNoEntitySelected -= ClearStatsDisplay;       
        }
        void UpdateStatsDisplay(SimpleEntity entity)
        {
            if (entity is Unit unit)
            {
                UpdateStatsDisplayUnit(unit);
            }
        }
        void ClearStatsDisplay()
        {
            if (statsText == null) return;
            statsText.text = "";
        }
        void UpdateStatsDisplayUnit(Unit unit)
        {
            if (statsText == null) return;
            
            ExtendedEntityStats stats = unit.GetStats();
            statsText.text = $"{unit.Data.GetName()} -> {stats.InitiativeStat}\n" +
                              $"HP -> {stats.CurrentHealthStat}\n" +
                              $"MaxHP -> {stats.MaxHealthStat}\n" +
                              $"attack -> {stats.AttackStat}\n" +
                              $"defense -> {stats.DefenseStat}\n" +
                              $"movement -> {stats.MovementStat}\n" +
                              $"price -> {stats.PriceStat}";
        }
    }
}