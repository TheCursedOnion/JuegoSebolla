using System;
using CursedOnion.Game.Commands;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Events;
using CursedOnion.Game.Handlers;
using CursedOnion.Game.Logic;
using CursedOnion.ScriptableObjects;
using Reflex.Attributes;
using TMPro;
using UnityEngine;

namespace CursedOnion.UI.Canvases.Level
{
    public class UnitInformationWindow : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI statsText;
        
        [Inject] LevelManager levelManager;
        EntityCommandHandler entityCommandHandler;
        
        void Awake()
        {
            entityCommandHandler = levelManager.CommandHandler;
        }
        private void OnEnable()
        {
            entityCommandHandler.OnEntitySelected += UpdateStatsDisplay;
        }
        private void OnDisable()
        {
            entityCommandHandler.OnEntitySelected -= UpdateStatsDisplay;
        }
        public void UpdateStatsDisplay(SimpleEntity entity)
        {
            if(entityCommandHandler.HasPreparedCommand()) return;
            
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