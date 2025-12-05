
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
using CursedOnion.Game.Logic.Services;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections;

namespace CursedOnion.Game.Systems.Level
{
    public class TurnSystem : MonoBehaviour
    {
        [SerializeField] private float delayOnAITurnEnd = 2.5f;
        [SerializeField] private List<Unit> allies = new List<Unit>();
        [SerializeField] private List<Unit> enemies = new List<Unit>();
        [SerializeField] private List<Unit> activeUnits = new List<Unit>();
        [SerializeField] private List<SimpleEntity> wallEntities = new List<SimpleEntity>();
        List<Unit> mergedUnits = new List<Unit>();
        public List<Unit> GetActiveUnits() => activeUnits;
        public List<Unit> GetAllyUnits() => allies;
        public List<Unit> GetEnemyUnits() => enemies;
        public List<Unit> GetMergedUnits() => mergedUnits;
        public List<SimpleEntity> GetWallEntities() => wallEntities;
        
        LevelEvents levelEvents;
        int currentInitiative;
        bool isNextAllyTurn = true;
        bool battleStarted = false;
        bool CanContinue() => allies.Count > 0 && enemies.Count > 0;
        

        public void Initialize(LevelEvents levelEvents)
        {
            this.levelEvents = levelEvents;
            levelEvents.OnLevelStateChange += TryToBegin;
            levelEvents.OnUnitTurnRegisterPetition += AddUnit;
            levelEvents.OnUnitTurnUnregisterPetition += RemoveUnit;
        }
        
        private void OnDisable()
        {
            levelEvents.OnLevelStateChange -= TryToBegin;
            levelEvents.OnUnitTurnRegisterPetition -= AddUnit;
            levelEvents.OnUnitTurnUnregisterPetition -= RemoveUnit;
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
            if (activeUnits.Contains(unit))
                activeUnits.Remove(unit);

            if (allies.Contains(unit))
                allies.Remove(unit);

            if (enemies.Contains(unit))
            {
                enemies.Remove(unit);
                if(unit.IsBoss) levelEvents.InvokeBossEnemyDeath();
            }

            if (mergedUnits.Contains(unit))
            {
        
                OrganizeLists();
                levelEvents.UpdateMergedUnitList(mergedUnits);
            }

            CheckForBattleEnd();
        }
        
        void TryToBegin(LevelState previousState, LevelState newState)
        {
            if(newState == LevelState.InBattle) BeginBattle();
        }
        public void BeginBattle()
        {
            battleStarted = true;
            isNextAllyTurn = true;
            
            OrganizeLists();

            ResetInitiative();
            ProcessTurn();
        }
        void OrganizeLists()
        { 
            if (allies.Count == 0 || enemies.Count == 0) return;
            
            allies = allies.OrderByDescending(u => u.Stats.InitiativeStat).ToList();
            enemies = enemies.OrderByDescending(u => u.Stats.InitiativeStat).ToList();
            
            mergedUnits.Clear();
            mergedUnits = allies.Concat(enemies)
                .OrderByDescending(u => u.Stats.InitiativeStat)
                .ThenBy(u => u.GetSide() == BattleSide.Enemy ? 1 : 0)
                .ToList();
            
            levelEvents.UpdateMergedUnitList(mergedUnits);
        }
        void ResetInitiative()
        {
            Debug.Log("======== NUEVA RONDA EMPIEZA ========");
            levelEvents.PassRound();
            
            var maxAllyInitiative = allies[0].Stats.InitiativeStat;
            var maxEnemyInitiative = enemies[0].Stats.InitiativeStat;
            currentInitiative = Mathf.Max(maxEnemyInitiative, maxAllyInitiative);
        }
        void ProcessTurn()
        {
            bool isAllyTurn = isNextAllyTurn;
            Debug.Log($"-- Iniciativa actual: {currentInitiative} | Para aliados? {isAllyTurn} --");
            
            isNextAllyTurn = !isNextAllyTurn;
            
            var source = isAllyTurn ? allies : enemies;
            var turnGroup = source
                .Where(u => u.Stats.InitiativeStat == currentInitiative)
                .ToList();

            bool handled = HandleGroup(turnGroup);

            if (!handled)
            {
                if (!isAllyTurn) MoveToNextInitiative();
                else ProcessTurn();
            }
            else
            {
                levelEvents.InvokeTurnBegin(isAllyTurn);
            }

            
        }
        void MoveToNextInitiative()
        {
            if (isNextAllyTurn)
            {
                currentInitiative--;
                if (currentInitiative == 0) ResetInitiative();
            }
            ProcessTurn();
        }
        bool HandleGroup(List<Unit> groupList)
        {
            if (groupList.Count == 0) return false;
            
            activeUnits.Clear();
            activeUnits.AddRange(groupList);

            
            
            foreach (var unit in activeUnits)
            {
                unit.NotifyStartTurn();
            }

            IEnumerable<Unit> unitsToProcess = !isNextAllyTurn 
                ? activeUnits 
                : activeUnits.Take(1);

            foreach (var unit in unitsToProcess)
            {
                unit.EntityController.ProcessTurn();
            }

            if(activeUnits.Count > 0)
                FocusOnStartingUnit();
            
            return true;
        }
        void FocusOnStartingUnit()
        {
            var unit = activeUnits[0];
            unit.FocusOnUnit();
        }

        public bool IsUnitActive(Unit unit) => activeUnits.Contains(unit);
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
            if (activeUnits.Count > 0)
            {
                activeUnits[0].EntityController.ProcessTurn();
                FocusOnStartingUnit();
            }
            else
            {
                StartCoroutine(DelayedEndTurn(delayOnAITurnEnd));
            }
        }

        IEnumerator DelayedEndTurn(float delay)
        {
            yield return new WaitForSeconds(delay);
            InvokeEndTurn();
        }

        void EndTurnForUnit(Unit unit)
        {
            if (activeUnits.Contains(unit)) activeUnits.Remove(unit);
        }

        private void InvokeEndTurn()
        {
            levelEvents.InvokeTurnEnd();

            if (CanContinue())
            {
                if (isNextAllyTurn)
                {
                    MoveToNextInitiative();
                }
                else
                {
                    ProcessTurn();
                }
            }
        }

        private void CheckForBattleEnd()
        {
            if (!battleStarted) return;
            
            Debug.Log($"Comprobando fin de batalla: Aliados restantes {allies.Count}, Enemigos restantes {enemies.Count}");
            if (allies.Count == 0)
            {
                levelEvents.InvokeAllAlliesDeath();
            }
            else if (enemies.Count == 0)
            {
                levelEvents.InvokeAllEnemiesDeath();
            }
            else if(activeUnits.Count == 0)
            {
                StartCoroutine(DelayedEndTurn(delayOnAITurnEnd));
            }
            
        }

        void Update()
        {
            /*if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Ha ganado el bando Enemigo");
                transitionData.Color = Color.red;
                sceneServiceUser.ChangeScene(resetScene, transitionData);
            }*/
        }

    }
}
