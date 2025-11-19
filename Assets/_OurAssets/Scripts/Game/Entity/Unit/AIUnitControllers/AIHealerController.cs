using System.Collections.Generic;
using UnityEngine;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Entity.Components;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.Game.Systems.Level;
using BehaviourAPI.UnityToolkit.GUIDesigner.Runtime;
using BehaviourAPI.Core;
using CursedOnion.Game.Commands;

namespace CursedOnion.Game.Entity
{
    public class AIHealerController : AIUnitController
    {
        public bool isWoundedClose;
        public bool isAllyCriticalClose;
        public bool dangerNearby;

        public SimpleEntity allyTarget;
        public SimpleEntity enemyTarget;

        public List<Vector3> healerReachableTiles = new();
        public List<Vector3> healerReachableAttackPositions = new();
        public List<Vector3> healerReachableHealPositions = new();
        public float tileSafetyScore;

        Unit unit;  
        LevelManager level;
        TurnSystem turn;
        AIUnitController baseAI;

        public override void Initialize(SimpleEntity entity, EntityComponents components)
        {
            base.Initialize(entity, components);

            unit = entity as Unit;
            level = entity.LevelManager;
            turn = entity.LevelManager.GetTurnSystem();
            baseAI = entity.GetComponent<AIUnitController>();
        }

        //PERCEPCIONES PRINCIPALES

        /// Detecta si hay aliados críticos (<25% HP) en rango de cura
        public bool DetectCriticalAlliesInRange()
        {
            var grid = unit.Grid;
            var position = unit.transform.position;
            grid.TryWorldToGridPosition(position, out Vector3 gridPos);

            isAllyCriticalClose = false;
            allyTarget = null;
            healerReachableHealPositions.Clear();

            AStarPathFinder.InsertMeleeAttackGridPositions(healerReachableHealPositions, grid, gridPos);

            // TEngo que cambiarlo para que use healerReachableHealPositions
            foreach (var ally in turn.GetEnemyUnits())
            {
                if (ally == unit) continue;

                float hpPercent = ally.Stats.CurrentHealthStat / (float)ally.Stats.MaxHealthStat;

                if (hpPercent < 0.25f && IsInHealingRange(ally))
                {
                    isAllyCriticalClose = true;
                    return isAllyCriticalClose;
                }
            }
            return isAllyCriticalClose;
        }

        /// Detecta aliados críticos fuera de rango (para moverse hacia ellos)
        public bool DetectCriticalAlliesFar()
        {
            foreach (var ally in turn.GetEnemyUnits())
            {
                if (ally == unit) continue;

                float hpPercent = ally.Stats.CurrentHealthStat / (float)ally.Stats.MaxHealthStat;

                if (hpPercent < 0.25f && !IsInHealingRange(ally))
                    return true;
            }
            return false;
        }

        /// Detecta si hay aliados heridos (<70% HP) cerca
        public bool DetectWoundedAlliesInRange()
        {
            isWoundedClose = false;
            allyTarget = null;

            foreach (var ally in turn.GetEnemyUnits())
            {
                if (ally == unit) continue;

                float hpPercent = ally.Stats.CurrentHealthStat / (float)ally.Stats.MaxHealthStat;

                if (hpPercent <= 0.70f && IsInHealingRange(ally))
                {
                    isWoundedClose = true;
                    return true;
                }
            }
            return false;
        }

        /// Detecta enemigos matables
        public bool DetectKillableEnemies()
        {
            enemyTarget = null;



            foreach (var enemy in turn.GetAllyUnits()) 
            {
                if (enemy.GetSide() == unit.GetSide()) continue;

                if (!IsEnemyInAttackRange(enemy)) continue;

                int damage = Mathf.Max(0, unit.Stats.AttackStat - enemy.Stats.DefenseStat);

                if (enemy.Stats.CurrentHealthStat <= damage)
                {
                    enemyTarget = enemy;
                    return true;
                }
            }
            return false;
        }

        // ACCIONES

        /// Acción: Curar aliado ya seleccionado
        public void Heal()
        {
            GetEntityComponent<SpecialAbilityComponent>().DoAbility(allyTarget, false);
        }

        public Status AttackKillableEnemy()
        {
            // ya vere
            return Status.Failure;
        }

        public Status MoveToUtilityTile()
        {
            return Status.Failure;
        }


        bool IsInHealingRange(Unit ally)
        {
            return false;
        }

        bool IsEnemyInAttackRange(SimpleEntity enemy)
        {
            return false; 
        }

    }
}
