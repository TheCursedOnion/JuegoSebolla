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
    public class AISoldierController : AIUnitController
    {
        public SimpleEntity allyHealer;

        List<Vector3> soldierReachableTiles = new();
        List<Vector3> soldierReachableAttackTiles = new();
        List<Vector3> enemyPositions = new();
        public List<Vector3> posCloseToHealers = new();

        AIUnitController baseAI;

        SimpleEntity enemyTarget;

        //garantiza que el baseAI esté listo
        private void LazyInit()
        {
            if (baseAI != null) return;

            baseAI = GetComponent<AIUnitController>();
            if (baseAI == null)
                Debug.LogError("AIHealerController NO encontró AIUnitController en el mismo GameObject.");
        }


        #region Perceptions

        public bool WeakAndHealerClose()
        {
            LazyInit();

            float healthPercent = AssignedEntity.Stats.CurrentHealthStat / AssignedEntity.Stats.MaxHealthStat;

            if (healthPercent >= 0.3f)
                return false;

            posCloseToHealers.Clear();

            var unit = baseAI.GetUnit();
            var grid = unit.Grid;

            var healers = baseAI.GetTurnSystem().GetAllyUnits().Where(u => u != unit && u.Stats.SpecialAbilityType is HealerAbility).ToList();

            if (healers.Count == 0)
                return false;

            soldierReachableTiles.Clear();
            _ = AStarPathFinder.InsertReachableGridPositionsAsyncBFS(
                soldierReachableTiles,
                grid,
                unit.transform.position,
                unit.Stats.MovementStat
            );

            foreach (var healer in healers)
            {
                var adjTiles = GetAdjacentTilesPos(healer);

                foreach (var tile in adjTiles)
                {
                    if (soldierReachableTiles.Contains(tile))
                    {
                        posCloseToHealers.Add(tile);
                    }
                }
            }

            return posCloseToHealers.Count > 0;
        }

        public bool EnemyInRange()
        {
            LazyInit();
            enemyPositions.Clear();
            soldierReachableAttackTiles.Clear();

            var unit = baseAI.GetUnit();
            var turn = baseAI.GetTurnSystem();
            var grid = unit.Grid;

            grid.TryWorldToGridPosition(unit.transform.position, out Vector3 gridPos);
            AStarPathFinder.InsertMeleeAttackGridPositions(soldierReachableAttackTiles, grid, gridPos);

            foreach (var enemy in turn.GetAllyUnits())
            {
                grid.TryWorldToGridPosition(enemy.transform.position, out Vector3 enemyGridPos);
                if (soldierReachableAttackTiles.Contains(enemyGridPos))
                    enemyPositions.Add(enemy.transform.position);

            }
            return enemyPositions.Count > 0;
        }

        public bool EnemyInMovementRange()
        {
            LazyInit();
            enemyPositions.Clear();
            soldierReachableTiles.Clear();

            var unit = baseAI.GetUnit();
            var turn = baseAI.GetTurnSystem();

            _ = AStarPathFinder.InsertReachableGridPositionsAsyncBFS(
                soldierReachableTiles,
                unit.Grid,
                unit.transform.position,
                unit.Stats.MovementStat
            );

            foreach (var enemy in turn.GetAllyUnits())
            {
                if (GetAdjacentTilesPos(enemy).Any(adj => soldierReachableTiles.Contains(adj)))
                {
                    enemyPositions.Add(enemy.transform.position);
                }
            }
            return enemyPositions.Count > 0;
        }

        public bool CanRage()
        {
            LazyInit();

            var unit = baseAI.GetUnit();
            var allies = baseAI.GetTurnSystem().GetAllyUnits();

            int totalNearby = 0;
            int alliesWhoUsedAbility = 0;

            foreach (var ally in allies)
            {
                if (ally == unit) continue; 

                float dist = Vector3.Distance(unit.transform.position, ally.transform.position);
                if (dist <= 5f)
                {
                    totalNearby++;

                    if (ally.ActionHandler.HasUsedAbility())
                        alliesWhoUsedAbility++;
                }
            }

            if (totalNearby == 0)
                return true;

            return alliesWhoUsedAbility <= (totalNearby / 2f);
        }


        //  TILE ADYACENTES

        private List<Vector3> GetAdjacentTilesPos(SimpleEntity entity)
        {
            LazyInit();

            var unit = baseAI.GetUnit();
            var grid = unit.Grid;

            var positions = new List<Vector3>();
            grid.TryWorldToGridPosition(entity.transform.position, out Vector3 gridPos);

            Vector3[] dirs =
            {
                new Vector3( 1, 0,0),
                new Vector3(-1, 0,0),
                new Vector3(0,0, 1),
                new Vector3(0,0,-1),
            };

            foreach (var d in dirs)
            {
                Vector3 pos = gridPos + d;
                if (grid.IsGridPositionInBounds(pos) &&
                    grid.GetTileAtGridPosition(pos).IsEmptyTile() &&
                    grid.GetTileAtGridPosition(pos).GetContainedEntity() == null)
                {
                    positions.Add(pos);
                }
            }

            return positions;
        }
        #endregion


        #region ActionLogic
        public Status EndAction() => Status.Success;

        public void Rage()
        {
            Debug.Log("Soldier is Raging");
            baseAI.GetEntityComponent<SpecialAbilityComponent>().DoAbility(AssignedEntity, false);
        }
        #endregion


        #region UtilitySystems

        public void SelectBestEnemyToAttack()
        {
            Debug.Log("hay estas posiciones de enemigos: " + enemyPositions.Count);
            LazyInit();

            var unit = baseAI.GetUnit();
            Unit best = null;
            float bestScore = float.MinValue;

            foreach (var pos in enemyPositions)
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
            Debug.Log("Soldier selected best enemy to attack: " + enemyTarget?.name);
        }

        public void SelectBestTileNearEnemy()
        {
            LazyInit();

            if (enemyTarget == null) return;

            var unit = baseAI.GetUnit();
            soldierReachableTiles.Clear();

            _ = AStarPathFinder.InsertReachableGridPositionsAsyncBFS(
                soldierReachableTiles,
                unit.Grid,
                unit.transform.position,
                unit.Stats.MovementStat
            );

            var adjacentTiles = GetAdjacentTilesPos(enemyTarget);
            var allEnemies = baseAI.GetTurnSystem().GetAllyUnits();

            Vector3 bestTile = Vector3.zero;
            float bestScore = float.MinValue;

            foreach (var t in adjacentTiles)
            {
                if (!soldierReachableTiles.Contains(t)) continue;

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
                Debug.Log("Soldier selected best tile near enemy: " + bestTile);
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
