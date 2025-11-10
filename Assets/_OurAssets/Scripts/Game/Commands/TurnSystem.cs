
using CursedOnion.Game.Commands;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Modes.General.UI.Transitions;
using CursedOnion.Game.Systems.Level;
using CursedOnion.ScriptableObjects;
using NaughtyAttributes;
using Reflex.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CursedOnion
{
    public class TurnSystem : MonoBehaviour
    {
        [Inject] private LevelAsset levelAsset;
        [Inject] private CommandManager commandManager;
        
        [BoxGroup("End Game"), Scene, SerializeField] private string resetScene;
        [BoxGroup("End Game"), SerializeField] UITransitionData transitionData;
        
        int currentInitiative;
        private bool alliesProcessedForCurrentInitiative = false;

        [SerializeField]private List<Unit> allies = new List<Unit>();
        [SerializeField] private List<Unit> enemies = new List<Unit>();

        [SerializeField] private List<Unit> activeUnits = new List<Unit>();
        public List<Unit> GetActiveUnits() => activeUnits;

        public event Action<Unit> OnUnitTurnStart;
        public event Action<Unit> OnUnitTurnEnd;

        public void AddUnit(Unit unit)
        {
            if (unit.Side == BattleSide.Enemy)
            {
                if(!enemies.Contains(unit)) enemies.Add(unit);
            }
            else if (unit.Side == BattleSide.Ally)
            {
                if (!allies.Contains(unit)) allies.Add(unit);
            }
        }
        public void RemoveUnit(Unit unit)
        {
            if(allies.Contains(unit)) allies.Remove(unit);
            if(enemies.Contains(unit)) enemies.Remove(unit);
        }

        public void StartRound()
        { 
            Debug.Log("======== NUEVA RONDA EMPIEZA ========");
            
            if (allies.Count == 0 || enemies.Count == 0) return;
            
            allies = allies.OrderByDescending(u => u.GetStats().InitiativeStat).ToList();
            enemies = enemies.OrderByDescending(u => u.GetStats().InitiativeStat).ToList();

            var maxAllyInitiative = allies[0].GetStats().InitiativeStat;
            var maxEnemyInitiative = enemies[0].GetStats().InitiativeStat;
            currentInitiative = Mathf.Max(maxEnemyInitiative, maxAllyInitiative);
            
            StartInitiativeGroup();
        }

        private void StartInitiativeGroup()
        {
            Debug.Log($"-- Iniciativa actual: {currentInitiative} --");
            commandManager.ClearStack();

            var allyGroup = allies.Where(u => u.GetStats().InitiativeStat == currentInitiative).ToList();
            var enemyGroup = enemies.Where(u => u.GetStats().InitiativeStat == currentInitiative).ToList();

            if (!alliesProcessedForCurrentInitiative && allyGroup.Count > 0)
            {
                activeUnits.Clear();
                activeUnits.AddRange(allyGroup);

                alliesProcessedForCurrentInitiative = true;

                foreach (var unit in allyGroup)
                {
                    OnUnitTurnStart?.Invoke(unit);
                    unit.UnitController.ProcessTurn(unit);
                }
                return;
            }

            if (enemyGroup.Count > 0)
            {
                activeUnits.Clear();
                activeUnits.AddRange(enemyGroup);

                foreach (var enemy in enemyGroup)
                {
                    OnUnitTurnStart?.Invoke(enemy);
                    enemy.UnitController.ProcessTurn(enemy);
                    OnUnitTurnEnd?.Invoke(enemy);
                }
            }
            
            alliesProcessedForCurrentInitiative = false;

            currentInitiative--;
            if (currentInitiative > 0)
                StartInitiativeGroup();
            else
                StartRound();
        }

        public void EndTurnForUnit(Unit unit)
        {
            if (unit == null || unit.GetStats().CurrentHealthStat <= 0)
            {
                if (activeUnits.Contains(unit))
                    activeUnits.Remove(unit);

                if (activeUnits.Count == 0)
                    StartInitiativeGroup();

                return;
            }

            if (activeUnits.Contains(unit))
            {
                activeUnits.Remove(unit);
                OnUnitTurnEnd?.Invoke(unit);
            }

            if (activeUnits.Count == 0)
            {
                StartInitiativeGroup();
            }
        }

    }
}
