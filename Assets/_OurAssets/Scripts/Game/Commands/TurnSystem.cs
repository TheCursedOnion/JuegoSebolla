
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
using CursedOnion.Game.Events;
using UnityEngine;

namespace CursedOnion.Game.Systems.Level
{
    public class TurnSystem : MonoBehaviour
    {
        private LevelEvents levelEvents;
        
        [BoxGroup("End Game"), Scene, SerializeField] private string resetScene;
        [BoxGroup("End Game"), SerializeField] UITransitionData transitionData;
        
        int currentInitiative;
        private bool alliesProcessedForCurrentInitiative = false;

        [SerializeField] private List<Unit> allies = new List<Unit>();
        [SerializeField] private List<Unit> enemies = new List<Unit>();

        [SerializeField] private List<Unit> activeUnits = new List<Unit>();
        public List<Unit> GetActiveUnits() => activeUnits;
        public List<Unit> GetAllyUnits() => allies;
        public List<Unit> GetEnemyUnits() => enemies;
        public void Initialize(LevelEvents levelEvents)
        {
            this.levelEvents = levelEvents;
            levelEvents.OnUnitTurnRegisterPetition += AddUnit;
        }

        private void OnDisable()
        {
            levelEvents.OnUnitTurnRegisterPetition -= AddUnit;
            foreach (var unit in activeUnits.ToList())
            {
                EndTurnForUnit(unit);
            }
        }

        void AddUnit(Unit unit)
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

        public void BeginBattle()
        {
            OrganizeLists();
        }
        void OrganizeLists()
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
            
            var turnGroup = !alliesProcessedForCurrentInitiative
                ? allies.Where(u => u.GetStats().InitiativeStat == currentInitiative).ToList()
                : enemies.Where(u => u.GetStats().InitiativeStat == currentInitiative).ToList();
            alliesProcessedForCurrentInitiative = !alliesProcessedForCurrentInitiative;

            if (turnGroup.Count > 0)
                HandleGroup(turnGroup);
            else
                MoveToNextTurn();
        }
        void MoveToNextTurn()
        {
            if (alliesProcessedForCurrentInitiative)
            {
                currentInitiative--;
                if (currentInitiative > 0)
                    StartInitiativeGroup();
                else
                    OrganizeLists();
            }
            else
            {
                StartInitiativeGroup();
            }
        }

        void HandleGroup(List<Unit> groupList)
        {
            if (groupList.Count > 0)
            {
                activeUnits.Clear();
                activeUnits.AddRange(groupList);

                foreach (var unit in groupList)
                {
                    unit.OnEntityUpdate += ProcessEntityUpdate;
                    unit.EntityController.ProcessTurn();
                }
            }
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
            foreach (var unit in activeUnits.ToList())
            {
                EndTurnForUnit(unit);
            }

            InvokeEndTurn();
        }
        public void EndTurnForAIUnit(Unit unit)
        {
            EndTurnForUnit(unit);
            if(activeUnits.Count == 0) InvokeEndTurn();
        }
        void EndTurnForUnit(Unit unit)
        {
            if (activeUnits.Contains(unit))
            {
                unit.OnEntityUpdate -= ProcessEntityUpdate;
                activeUnits.Remove(unit);
            }
        }
        private void InvokeEndTurn()
        {
            levelEvents.InvokeTurnEnd();
            
            if(allies.Count > 0 && enemies.Count > 0)
                MoveToNextTurn();
        }

    }
}
