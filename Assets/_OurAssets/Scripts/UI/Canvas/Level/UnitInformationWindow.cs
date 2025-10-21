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

        public void UpdateStatsDisplay(IEntity entity)
        {
            if (entity == null)
            {
                ClearStatsDisplay();
                return;
            }
            
            if (entity is Character character)
            {
                UpdateStatsDisplayCharacter(character);
            }
        }
        void ClearStatsDisplay()
        {
            if (statsText == null) return;
            statsText.text = "";
        }
        void UpdateStatsDisplayCharacter(Character character)
        {
            if (statsText == null) return;
            
            EntityStats stats = character.Stats;
            statsText.text = $"{character.Name} -> {stats.InitiativeStat}\n" +
                              $"HP -> {stats.CurrentHealthStat}\n" +
                              $"MaxHP -> {stats.MaxHealthStat}\n" +
                              $"attack -> {stats.AttackStat}\n" +
                              $"defense -> {stats.DefenseStat}\n" +
                              $"movement -> {stats.MovementStat}\n" +
                              $"price -> {stats.PriceStat}";
        }
    }
}