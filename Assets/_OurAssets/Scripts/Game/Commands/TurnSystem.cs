
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

namespace CursedOnion.Game.Systems.Level
{
    public class TurnSystem : MonoBehaviour
    {
        private LevelEvents levelEvents;
        
        [BoxGroup("End Game"), Scene, SerializeField] private string resetScene;
        [BoxGroup("End Game"), SerializeField] UITransitionData transitionData;
        [BoxGroup("End Game"), SerializeField] SceneServiceUser sceneServiceUser;
        
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
            levelEvents.OnLevelStateChange += TryToBegin;
            levelEvents.OnUnitTurnRegisterPetition += AddUnit;
            levelEvents.OnUnitTurnUnregisterPetition += RemoveUnit;
        }

        void TryToBegin(LevelState previousState, LevelState newState)
        {
            if(newState == LevelState.InBattle) BeginBattle();
        }
        public void BeginBattle()
        {
            OrganizeLists();
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
                enemies.Remove(unit);

            CheckForBattleEnd();
        }
        void OrganizeLists()
        { 
            Debug.Log("======== NUEVA RONDA EMPIEZA ========");
            if (allies.Count == 0 || enemies.Count == 0) return;
            
            allies = allies.OrderByDescending(u => u.Stats.InitiativeStat).ToList();
            enemies = enemies.OrderByDescending(u => u.Stats.InitiativeStat).ToList();

            var maxAllyInitiative = allies[0].Stats.InitiativeStat;
            var maxEnemyInitiative = enemies[0].Stats.InitiativeStat;
            currentInitiative = Mathf.Max(maxEnemyInitiative, maxAllyInitiative);
            
            StartInitiativeGroup();
        }
        //private void StartTurnFor(List<Unit> )
        void StartInitiativeGroup()
        {
            Debug.Log($"-- Iniciativa actual: {currentInitiative} --");
            
            var turnGroup = !alliesProcessedForCurrentInitiative
                ? allies.Where(u => u.Stats.InitiativeStat == currentInitiative).ToList()
                : enemies.Where(u => u.Stats.InitiativeStat == currentInitiative).ToList();
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
            if (groupList.Count == 0) return;
            
            activeUnits.Clear();
            activeUnits.AddRange(groupList);

            bool groupIsAllies = groupList[0].GetSide() == BattleSide.Ally;

            if (groupIsAllies)
            {
                foreach (var unit in activeUnits)
                    unit.EntityController.ProcessTurn();

                ChooseStartingUnit();
            }
            else
            {
                ChooseStartingEnemyUnit();
            }
            
            levelEvents.InvokeTurnBegin(alliesProcessedForCurrentInitiative);
        }
        void ChooseStartingUnit()
        {
            //int randomIndex = Random.Range(0, activeUnits.Count);
            levelEvents.InvokeTurnFocus(activeUnits[0]);
        }

        void ChooseStartingEnemyUnit()
        {
            levelEvents.InvokeTurnFocus(activeUnits[0]);
            activeUnits[0].EntityController.ProcessTurn();
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
                ChooseStartingEnemyUnit();
            }
            else
            {
                InvokeEndTurn();
            }
        }

        void EndTurnForUnit(Unit unit)
        {
            if (activeUnits.Contains(unit)) activeUnits.Remove(unit);
        }

        private void InvokeEndTurn()
        {
            levelEvents.InvokeTurnEnd();
            
            if(allies.Count > 0 && enemies.Count > 0)
                MoveToNextTurn();
        }

        private void CheckForBattleEnd()
        {
            Debug.Log($"Comprobando fin de batalla: Aliados restantes {allies.Count}, Enemigos restantes {enemies.Count}");
            if (allies.Count == 0)
            {
                Debug.Log("Ha ganado el bando Enemigo");
                transitionData.Color = Color.red;
                sceneServiceUser.ChangeScene(resetScene, transitionData);
            }
            if (enemies.Count == 0)
            {
                Debug.Log("Ha ganado el bando Aliado");
                transitionData.Color = Color.blue;
                sceneServiceUser.ChangeScene(resetScene, transitionData);
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
