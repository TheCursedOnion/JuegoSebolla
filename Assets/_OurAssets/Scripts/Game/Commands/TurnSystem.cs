
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

        [SerializeField] private List<Unit> allies = new List<Unit>();
        [SerializeField] private List<Unit> enemies = new List<Unit>();

        [SerializeField] private List<Unit> activeUnits = new List<Unit>();
        public List<Unit> GetActiveUnits() => activeUnits;

        public event Action OnTurnStart;
        public event Action OnTurnEnd;

        public void AddUnit(Unit unit)
        {
            if (unit.GetSide() == BattleSide.Enemy)
            {
                if(!enemies.Contains(unit)) enemies.Add(unit);
            }
            else if (unit.GetSide()  == BattleSide.Ally)
            {
                if (!allies.Contains(unit)) allies.Add(unit);
            }
        }
        public void RemoveUnit(Unit unit)
        {
            if(allies.Contains(unit)) allies.Remove(unit);
            if(enemies.Contains(unit)) enemies.Remove(unit);
        }

        public void OrganizeLists()
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
        
        //private void StartTurnFor(List<Unit> )

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
                    unit.OnEntityUpdate += ProcessEntityUpdate;
                    unit.EntityController.ProcessTurn();
                }
                return;
            }

            if (enemyGroup.Count > 0)
            {
                activeUnits.Clear();
                activeUnits.AddRange(enemyGroup);

                foreach (var enemy in enemyGroup)
                {
                    enemy.OnEntityUpdate += ProcessEntityUpdate;
                    enemy.EntityController.ProcessTurn();
                }
            }
            
            alliesProcessedForCurrentInitiative = false;

            currentInitiative--;
            if (currentInitiative > 0)
                StartInitiativeGroup();
            else
                OrganizeLists();
        }
        
        void ProcessEntityUpdate(SimpleEntity entity)
        {
            if(entity is not Unit unit) return;
            
            if (unit.GetFlags().HasDied())
            {
                EndTurnForUnit(unit);
                
                if(allies.Contains(unit)) allies.Remove(unit);
                if(enemies.Contains(unit)) enemies.Remove(unit);
            }
        }

        public void EndTurn()
        {
            if (activeUnits == null || activeUnits.Count == 0)
            {
                Debug.Log("No hay unidades activas.");
                return;
            }

            foreach (var unit in activeUnits.ToList())
            {
                EndTurnForUnit(unit);
            }
            OnTurnEnd?.Invoke();
        }

        public void EndTurnForUnit(Unit unit)
        {
            if (activeUnits.Contains(unit))
            {
                unit.OnEntityUpdate -= ProcessEntityUpdate;
                activeUnits.Remove(unit);
                
                if(activeUnits.Count == 0) StartInitiativeGroup();
            }
        }

    }
}
