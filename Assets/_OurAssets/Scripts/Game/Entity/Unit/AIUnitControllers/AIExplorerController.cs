using BehaviourAPI.Core;
using BehaviourAPI.UnityToolkit.Demos;
using CursedOnion.Extensions;
using CursedOnion.Game.Entity.Components;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.Game.Systems.Level;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CursedOnion.Game.Entity
{
    public class AIExplorerController : MonoBehaviour
    {
        public SimpleEntity allyHealer;

        List<Vector3> explorerReachableTiles = new();
        List<Vector3> explorerReachableAttackTiles = new();
        List<Vector3> posCloseToHealers = new();

        AIUnitController baseAI;

        SimpleEntity enemyTarget;

        //garantiza que el baseAI esté listo
        private void LazyInit()
        {
            if (baseAI != null) return;

            baseAI = GetComponent<AIUnitController>();
            if (baseAI == null)
                Debug.LogError("AIExplorerController NO encontró AIUnitController en el mismo GameObject.");
        }


        #region Perceptions

        public bool LowHealth()
        {
            LazyInit();
            float healthPercent = baseAI.GetUnit().Stats.CurrentHealthStat / baseAI.GetUnit().Stats.MaxHealthStat;
            return healthPercent < 0.3f;
        }

        public bool EnemyInSprintRange()
        {
            LazyInit();

            var unit = baseAI.GetUnit();
            if (unit == null || unit.Grid == null) return false;

            var grid = unit.Grid;

            List<Vector3> tmpEnemyPositions = new();

            List<Vector3> sprintReachableTiles = new();
            _ = AStarPathFinder.InsertReachableGridPositionsAsyncBFS(
                sprintReachableTiles,
                grid,
                unit.transform.position,
                unit.Stats.MovementStat + 2 // Sprint range
            );

            foreach (var enemy in baseAI.GetTurnSystem().GetAllyUnits())
            {
                var adjTiles = baseAI.GetAdjacentTilesToMove(enemy);

                if (adjTiles.Any(t => sprintReachableTiles.Contains(t)))
                {
                    tmpEnemyPositions.Add(enemy.transform.position);
                }
            }

            if (tmpEnemyPositions.Count > 0)
            {
                baseAI.enemyPositions.Clear();
                baseAI.enemyPositions.AddRange(tmpEnemyPositions);
                return true;
            }

            return false;
        }


        public bool HealerInRange()
        {
            LazyInit();
            baseAI.enemyPositions.Clear();
            explorerReachableAttackTiles.Clear();

            var unit = baseAI.GetUnit();
            var turn = baseAI.GetTurnSystem();
            var grid = unit.Grid;

            grid.TryWorldToGridPosition(unit.transform.position, out Vector3 gridPos);
            AStarPathFinder.InsertMeleeAttackGridPositions(explorerReachableAttackTiles, grid, gridPos);

            foreach (var healer in baseAI.GetTurnSystem().GetEnemyUnits().Where(u => u.Stats.SpecialAbilityType is HealerAbility))
            {
                grid.TryWorldToGridPosition(healer.transform.position, out Vector3 allyGridPos);
                if (explorerReachableAttackTiles.Contains(allyGridPos))
                    return true;

            }
            return false;
        }

        public bool HealerInMovementRange()
        {
            LazyInit();

            posCloseToHealers.Clear();

            var unit = baseAI.GetUnit();
            var grid = unit.Grid;

            var healers = baseAI.GetTurnSystem().GetEnemyUnits().Where(u => u != unit && u.Stats.SpecialAbilityType is HealerAbility).ToList();

            Debug.Log("Healers found: " + healers.Count);

            if (healers.Count == 0)
                return false;

            explorerReachableTiles.Clear();
            _ = AStarPathFinder.InsertReachableGridPositionsAsyncBFS(
                explorerReachableTiles,
                grid,
                unit.transform.position,
                unit.Stats.MovementStat
            );

            foreach (var healer in healers)
            {
                var adjTiles = baseAI.GetAdjacentTilesToMove(healer);

                foreach (var tile in adjTiles)
                {
                    if (explorerReachableTiles.Contains(tile))
                    {
                        posCloseToHealers.Add(tile);
                    }
                }
            }

            return posCloseToHealers.Count > 0;
        }

        public bool HealerInSprintRange()
        {
            LazyInit();

            posCloseToHealers.Clear();

            var unit = baseAI.GetUnit();
            var grid = unit.Grid;

            var healers = baseAI.GetTurnSystem().GetEnemyUnits()
                            .Where(u => u.Stats.SpecialAbilityType is HealerAbility)
                            .ToList();

            if (healers.Count == 0)
                return false;

            List<Vector3> sprintReachableTiles = new();
            _ = AStarPathFinder.InsertReachableGridPositionsAsyncBFS(
                sprintReachableTiles,
                grid,
                unit.transform.position,
                unit.Stats.MovementStat + 2 // Sprint range
            );

            foreach (var healer in healers)
            {
                var adjTiles = baseAI.GetAdjacentTilesToMove(healer);

                if (adjTiles.Any(t => sprintReachableTiles.Contains(t)))
                {
                    // Guardamos las tiles válidas para usar luego
                    foreach (var tile in adjTiles.Where(t => sprintReachableTiles.Contains(t)))
                        posCloseToHealers.Add(tile);

                    return true;
                }
            }

            return false;
        }

        #endregion

        #region ActionLogic

        public void Sprint()
        {
            Debug.Log("Explorer is Sprinting");
            var entity = baseAI.GetUnit() as SimpleEntity;
            baseAI.GetEntityComponent<SpecialAbilityComponent>().DoAbility(entity, false);
        }

        #endregion

        #region UtilitySystems

        public void SelectBestEnemyToAttack()
        {
            LazyInit();
            Debug.Log("hay estas posiciones de enemigos: " + baseAI.enemyPositions.Count);

            var unit = baseAI.GetUnit();
            Unit best = null;
            float bestScore = float.MinValue;

            foreach (var pos in baseAI.enemyPositions)
            {
                Tile3d tile = unit.Grid.GetTileAtWorldPosition(pos);
                if (tile?.GetContainedEntity() is not Unit enemy)
                    continue;

                float hpScore = 1f - (enemy.Stats.CurrentHealthStat / (float)enemy.Stats.MaxHealthStat);

                float typeScore = enemy.Stats.SpecialAbilityType switch
                {
                    HealerAbility _ => 1f,
                    ArcherAbility _ => 0.9f,
                    SoldierAbility _ => 0.7f,
                    ThiefAbility _ => 0.7f,
                    BarbarianAbility _ => 0.6f,
                    TankAbility _ => 0.4f,
                    ExplorerAbility _ => 0.5f,
                    _ => 0.5f
                };

                float score = hpScore + typeScore;

                if (score > bestScore)
                {
                    bestScore = score;
                    best = enemy;
                }
            }

            enemyTarget = best;
            baseAI.TargetedEnemy = enemyTarget;
            Debug.Log("Explorer selected best enemy to attack: " + enemyTarget?.name);
        }

        public void SelectBestTileNearEnemy()
        {
            LazyInit();

            if (enemyTarget == null) return;

            var unit = baseAI.GetUnit();
            explorerReachableTiles.Clear();

            Debug.Log("Explorer HA USADO HABILIDAD????" + baseAI.GetUnit().ActionHandler.HasUsedAbility());

            if (baseAI.GetUnit().ActionHandler.HasUsedAbility())
            {
                _ = AStarPathFinder.InsertReachableGridPositionsAsyncBFS(
                    explorerReachableTiles,
                    unit.Grid,
                    unit.transform.position,
                    unit.Stats.MovementStat + 2
                );
            }
            else
            {
                _ = AStarPathFinder.InsertReachableGridPositionsAsyncBFS(
                    explorerReachableTiles,
                    unit.Grid,
                    unit.transform.position,
                    unit.Stats.MovementStat // Sprint range
                );
            }

            var adjacentTiles = baseAI.GetAdjacentTilesToMove(enemyTarget);
            var allEnemies = baseAI.GetTurnSystem().GetAllyUnits();

            Vector3 bestTile = Vector3.zero;
            float bestScore = float.MinValue;

            foreach (var t in adjacentTiles)
            {
                if (!explorerReachableTiles.Contains(t)) continue;

                float avgDistToEnemies = allEnemies
                    .Select(e => Vector3.Distance(t, e.transform.position))
                    .DefaultIfEmpty(0f)
                    .Average();

                float score = avgDistToEnemies;

                if (score > bestScore)
                {
                    bestScore = score;
                    bestTile = t;
                }
            }

            if (bestScore == float.MinValue)
                return;

            baseAI.TargetedGridPosToMove = bestTile;

            if (unit.Grid.TryGridToWorldPosition(bestTile, out Vector3 targetWorld))
            {
                baseAI.TargetedPosToMove = targetWorld.CenterOnTile();
                Debug.Log("Explorer selected best tile near enemy: " + bestTile);
            }
        }

        public void SelectBestTileNearHealer()
        {
            LazyInit();

            var unit = baseAI.GetUnit();
            var grid = unit.Grid;

            float bestScore = float.MinValue;
            Vector3 bestTile = Vector3.zero;

            foreach (var tile in posCloseToHealers)
            {
                float score = 0f;

                foreach (var enemy in baseAI.GetTurnSystem().GetAllyUnits())
                {
                    float dist = Vector3.Distance(tile, enemy.transform.position);

                    if (dist <= 1.1f)
                        score -= 10f;

                    score += dist * 0.5f;
                }

                if (score > bestScore)
                {
                    bestScore = score;
                    bestTile = tile;
                }
            }

            if (bestTile == Vector3.zero)
                return;

            baseAI.TargetedGridPosToMove = bestTile;
            grid.TryGridToWorldPosition(bestTile, out Vector3 worldPos);
            baseAI.TargetedPosToMove = worldPos.CenterOnTile();
        }

        #endregion
    }
}
