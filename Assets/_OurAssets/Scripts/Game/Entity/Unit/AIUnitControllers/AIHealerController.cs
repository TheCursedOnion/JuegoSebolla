using BehaviourAPI.Core;
using BehaviourAPI.UnityToolkit.GUIDesigner.Runtime;
using CursedOnion.Game.Commands;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Entity.Components;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.Game.Systems.Level;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;

namespace CursedOnion.Game.Entity
{
    public class AIHealerController : AIUnitController
    {
        public SimpleEntity allyTarget;

        List<Vector3> healerReachableTiles = new();
        List<Vector3> healerReachableAttackPositions = new();
        List<Vector3> healerReachableHealPositions = new();
        public List<Vector3> criticalAlliesPos = new();
        public List<Vector3> woundedAlliesPos = new();

        Unit unit;  
        TurnSystem turn;
        AIUnitController baseAI;

        public override void Initialize(SimpleEntity entity, EntityComponents components)
        {
            base.Initialize(entity, components);

            unit = entity as Unit;
            turn = entity.LevelManager.GetTurnSystem();
            baseAI = entity.GetComponent<AIUnitController>();
        }

        #region Percepciones Principales

        //Detecta si hay aliados críticos (<25% HP) en rango de cura
        public bool DetectCriticalAlliesInHealRange()
        {
            criticalAlliesPos.Clear();
            healerReachableHealPositions.Clear();

            var grid = unit.Grid;
            var position = unit.transform.position;
            grid.TryWorldToGridPosition(position, out Vector3 gridPos);

            AStarPathFinder.InsertMeleeAttackGridPositions(healerReachableHealPositions, grid, gridPos);

            foreach (var ally in turn.GetEnemyUnits())
            {
                if (ally == unit) continue;

                float hpPercent = ally.Stats.CurrentHealthStat / (float)ally.Stats.MaxHealthStat;

                if (hpPercent < 0.25f)
                {
                    grid.TryWorldToGridPosition(ally.transform.position, out Vector3 allyGridPos);

                    if (healerReachableHealPositions.Contains(allyGridPos))
                    {
                        criticalAlliesPos.Add(allyGridPos);
                    }
                }
            }
            return criticalAlliesPos.Count > 0;
        }

        // Detecta aliados críticos fuera de rango (para moverse hacia ellos)
        public bool DetectCriticalAlliesFar()
        {
            criticalAlliesPos.Clear();
            healerReachableTiles.Clear();

            _ = AStarPathFinder.InsertReachableGridPositionsAsyncBFS(
                healerReachableTiles,
                unit.Grid,
                unit.transform.position,
                unit.Stats.MovementStat
            );

            foreach (var ally in turn.GetEnemyUnits())
            {
                if (ally == unit) continue;

                float hpPercent = ally.Stats.CurrentHealthStat / (float)ally.Stats.MaxHealthStat;

                if (hpPercent < 0.25f)
                {
                    // Obtener los tiles adyacentes al aliado
                    var adjacentTiles = GetAdjacentTilesPos(ally);

                    // Ver si alguna de esas casillas es alcanzable
                    foreach (var tilePos in adjacentTiles)
                    {
                        if (healerReachableTiles.Contains(tilePos))
                        {
                            criticalAlliesPos.Add(tilePos);
                        }
                    }
                }
            }
            return criticalAlliesPos.Count > 0;
        }

        // Detecta si hay aliados heridos (<70% HP) cerca
        public bool DetectWoundedAlliesInHealRange()
        {
            woundedAlliesPos.Clear();
            healerReachableHealPositions.Clear();

            var grid = unit.Grid;
            var position = unit.transform.position;
            grid.TryWorldToGridPosition(position, out Vector3 gridPos);

            AStarPathFinder.InsertMeleeAttackGridPositions(healerReachableHealPositions, grid, gridPos);

            foreach (var ally in turn.GetEnemyUnits())
            {
                if (ally == unit) continue;

                float hpPercent = ally.Stats.CurrentHealthStat / (float)ally.Stats.MaxHealthStat;

                if (hpPercent < 0.70f)
                {
                    grid.TryWorldToGridPosition(ally.transform.position, out Vector3 allyGridPos);

                    if (healerReachableHealPositions.Contains(allyGridPos))
                    {
                        woundedAlliesPos.Add(allyGridPos);
                    }
                }
            }
            return woundedAlliesPos.Count > 0;
        }

        public bool DetectWoundedAlliesFar()
        {
            woundedAlliesPos.Clear();
            healerReachableTiles.Clear();

            _ = AStarPathFinder.InsertReachableGridPositionsAsyncBFS(
                healerReachableTiles,
                unit.Grid,
                unit.transform.position,
                unit.Stats.MovementStat
            );

            foreach (var ally in turn.GetEnemyUnits())
            {
                if (ally == unit) continue;

                float hpPercent = ally.Stats.CurrentHealthStat / (float)ally.Stats.MaxHealthStat;

                if (hpPercent < 0.70f)
                {
                    // Obtener los tiles adyacentes al aliado
                    var adjacentTiles = GetAdjacentTilesPos(ally);

                    // Ver si alguna de esas casillas es alcanzable
                    foreach (var tilePos in adjacentTiles)
                    {
                        if (healerReachableTiles.Contains(tilePos))
                        {
                            woundedAlliesPos.Add(tilePos);
                        }
                    }
                }
            }
            return woundedAlliesPos.Count > 0;
        }

        // Detecta enemigos matables
        public bool DetectKillableEnemies()
        {
            healerReachableAttackPositions.Clear();

            var grid = unit.Grid;
            var position = unit.transform.position;
            grid.TryWorldToGridPosition(position, out Vector3 gridPos);

            AStarPathFinder.InsertMeleeAttackGridPositions(
                healerReachableAttackPositions,
                grid,
                gridPos
            );

            foreach (var pos in healerReachableAttackPositions)
            {
                Tile3d tile = grid.GetTileAtGridPosition(pos);
                if (tile == null) continue;

                SimpleEntity entity = tile.GetContainedEntity();
                if (entity == null) continue;

                if (entity.GetSide() == unit.GetSide())
                    continue;

                Unit enemy = entity as Unit;
                if (enemy == null) continue;

                int myDamage = unit.Stats.AttackStat;
                int enemyDefense = enemy.Stats.DefenseStat;
                int enemyHP = enemy.Stats.CurrentHealthStat;

                int finalDamage = Mathf.Max(0, myDamage - enemyDefense);

                if (finalDamage >= enemyHP)
                {
                    // MUERTE ASEGURADA
                    baseAI.TargetedEnemy = enemy;
                    return true;
                }
            }
            return false;
        }

        private List<Vector3> GetAdjacentTilesPos(Unit ally)
        {
            var tiles = new List<Vector3>();
            var grid = unit.Grid;

            grid.TryWorldToGridPosition(ally.transform.position, out Vector3 allyGridPos);

            Vector3[] directions =
            {
                new Vector3( 1, 0, 0),
                new Vector3(-1, 0, 0),
                new Vector3( 0, 0, 1),
                new Vector3( 0, 0,-1)
            };

            foreach (var dir in directions)
            {
                Vector3 pos = allyGridPos + dir;

                if (grid.IsGridPositionInBounds(pos) && grid.GetTileAtGridPosition(pos).IsEmptyTile() == true && grid.GetTileAtGridPosition(pos).GetContainedEntity() == null) // o tu propia comprobación
                    tiles.Add(pos);
            }

            return tiles;
        }

        #endregion

        #region Acciones Principales Healer

        // End Action
        public Status EndAction()
        {
            return Status.Success;
        }


        // Acción: Curar aliado ya seleccionado
        public void Heal()
        {
            GetEntityComponent<SpecialAbilityComponent>().DoAbility(allyTarget, false);
        }
        #endregion

        #region UtiltitySystems

        public void SelectBestCriticalAlly()
        {
            SelectBestAlly(criticalAlliesPos);
        }

        public void SelectBestWoundedAlly()
        {
            SelectBestAlly(woundedAlliesPos);
        }


        public void SelectBestAlly(List<Vector3> allyPositions)
        {
            Unit bestAlly = null;
            float bestScore = float.MinValue;

            foreach (var pos in allyPositions)
            {
                Tile3d tile = unit.Grid.GetTileAtGridPosition(pos);
                if (tile == null || tile.GetContainedEntity() == null) continue;

                var ally = tile.GetContainedEntity() as Unit;
                if (ally == null) continue;

                float hpScore = 1f - (ally.Stats.CurrentHealthStat / (float)ally.Stats.MaxHealthStat);

                float typeScore = ally.Stats.SpecialAbilityType switch
                {
                    TankAbility _ => 1.0f,
                    SoldierAbility _ => 0.9f,
                    ArcherAbility _ => 0.8f,
                    HealerAbility _ => 0.7f,
                    ThiefAbility _ => 0.6f,
                    BarbarianAbility _ => 0.5f,
                    ExplorerAbility _ => 0.4f,
                    _ => 0.5f
                };

                float score = hpScore + typeScore;

                if (score > bestScore)
                {
                    bestScore = score;
                    bestAlly = ally;
                }
            }

            if (bestAlly != null)
            {
                allyTarget = bestAlly;
            }
            
            return;
        }

        public void SelectBestTileNearTargetAlly()
        {
            if (allyTarget == null)
                return;

            healerReachableTiles.Clear();
            _ = AStarPathFinder.InsertReachableGridPositionsAsyncBFS(
                healerReachableTiles,
                unit.Grid,
                unit.transform.position,
                unit.Stats.MovementStat
            );

            List<Vector3> adjacentTiles = GetAdjacentTilesPos(allyTarget as Unit);

            Vector3 bestTile = Vector3.zero;
            float bestDistance = float.MaxValue;

            foreach (var tilePos in adjacentTiles)
            {
                if (!healerReachableTiles.Contains(tilePos)) continue;

                float distance = Vector3.Distance(unit.transform.position, tilePos);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestTile = tilePos;
                }
            }

            if (bestDistance == float.MaxValue)
                return; 

            baseAI.TargetedGridPosToMove = bestTile;
            unit.Grid.TryGridToWorldPosition(bestTile, out baseAI.TargetedPosToMove);
        }

        public void SelectSafestTile()
        {
            healerReachableTiles.Clear();

            _ = AStarPathFinder.InsertReachableGridPositionsAsyncBFS(
                healerReachableTiles,
                unit.Grid,
                unit.transform.position,
                unit.Stats.MovementStat
            );

            if (healerReachableTiles.Count == 0)
                return;

            var allies = turn.GetEnemyUnits().Where(a => a != unit).ToList();
            var enemies = turn.GetAllyUnits().Where(e => e != unit).ToList();

            Vector3 bestTile = Vector3.zero;
            float bestScore = float.MinValue;

            foreach (var tile in healerReachableTiles)
            {
                float score = 0f;

                // --- 1) Proximidad a aliados (cuanto más cerca mejor)
                float allyScore = 0f;
                foreach (var ally in allies)
                {
                    unit.Grid.TryWorldToGridPosition(ally.transform.position, out Vector3 allyPos);
                    float dist = Vector3.Distance(tile, allyPos);
                    allyScore += Mathf.Clamp(10f / (dist + 1f), 0f, 10f); 
                }

                // --- 2) Lejanía a enemigos (cuanto más lejos mejor)
                float enemyScore = 0f;
                foreach (var enemy in enemies)
                {
                    unit.Grid.TryWorldToGridPosition(enemy.transform.position, out Vector3 enemyPos);
                    float dist = Vector3.Distance(tile, enemyPos);
                    enemyScore += Mathf.Clamp(dist, 0f, 10f); 
                }

                //
                int enemiesClose = enemies.Count(enemy =>
                {
                    unit.Grid.TryWorldToGridPosition(enemy.transform.position, out Vector3 pos);
                    return Vector3.Distance(tile, pos) < 3f; // rango de peligro
                });

                float dangerPenalty = enemiesClose * 5f;

                //calcular puntuacion de tile
                score = allyScore + enemyScore - dangerPenalty;

                if (score > bestScore)
                {
                    bestScore = score;
                    bestTile = tile;
                }
            }

            if (bestScore == float.MinValue)
                return;

            baseAI.TargetedGridPosToMove = bestTile;
            unit.Grid.TryGridToWorldPosition(bestTile, out baseAI.TargetedPosToMove);

            return;
        }

        #endregion
    }
}
