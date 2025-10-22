using System;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Events;
using CursedOnion.Game.Logic;
using Reflex.Attributes;
using TMPro;
using UnityEngine;

namespace CursedOnion.UI.Canvases.Level
{
    public class UnitInformationWindow : MonoBehaviour
    {
        [Inject] LevelEvents levelEvents;
        [SerializeField] TextMeshProUGUI statsText;

        private void OnEnable()
        {
            levelEvents.OnEntityInspected += UpdateStatsDisplay;
        }
        private void OnDisable()
        {
            levelEvents.OnEntityInspected -= UpdateStatsDisplay;
        }

        public void UpdateStatsDisplay(SimpleEntity entity)
        {
            if (entity == null)
            {
                ClearStatsDisplay();
                return;
            }
            
            if (entity is Unit character)
            {
                UpdateStatsDisplayCharacter(character);
            }
        }
        void ClearStatsDisplay()
        {
            if (statsText == null) return;
            statsText.text = "";
        }
        void UpdateStatsDisplayCharacter(Unit unit)
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