using BehaviourAPI.Core;
using CursedOnion.Extensions;
using CursedOnion.Game.Entity.Components;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.Game.Systems.Level;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CursedOnion.Game.Entity
{
    public class AIHealerController : AIUnitController
    {
        public SimpleEntity allyTarget;

        List<Vector3> healerReachableTiles = new();
        List<Vector3> healerReachableHealPositions = new();
        List<Vector3> healerReachableAttackPositions = new();
        public List<Vector3> criticalAlliesPos = new();
        public List<Vector3> woundedAlliesPos = new();

        AIUnitController baseAI;

        //garantiza que el baseAI esté listo
        private void LazyInit()
        {
            if (baseAI != null) return;

            baseAI = GetComponent<AIUnitController>();
            if (baseAI == null)
                Debug.LogError("AIHealerController NO encontró AIUnitController en el mismo GameObject.");
        }

        
        #region Perceptions

        public bool DetectCriticalAlliesInHealRange()
        {
            LazyInit();
            criticalAlliesPos.Clear();
            healerReachableHealPositions.Clear();

            var unit = baseAI.GetUnit();
            var turn = baseAI.GetTurnSystem();
            var grid = unit.Grid;

            grid.TryWorldToGridPosition(unit.transform.position, out Vector3 gridPos);
            AStarPathFinder.InsertMeleeAttackGridPositions(healerReachableHealPositions, grid, gridPos);

            foreach (var ally in turn.GetEnemyUnits().Where(a => a != unit))
            {
                float hpPercent = ally.Stats.CurrentHealthStat / (float)ally.Stats.MaxHealthStat;

                if (hpPercent < 0.25f)
                {
                    grid.TryWorldToGridPosition(ally.transform.position, out Vector3 allyGridPos);
                    if (healerReachableHealPositions.Contains(allyGridPos))
                        criticalAlliesPos.Add(ally.transform.position);
                }
            }
            return criticalAlliesPos.Count > 0;
        }

        public bool DetectCriticalAlliesFar()
        {
            LazyInit();
            criticalAlliesPos.Clear();
            healerReachableTiles.Clear();

            var unit = baseAI.GetUnit();
            var turn = baseAI.GetTurnSystem();

            _ = AStarPathFinder.InsertReachableGridPositionsAsyncBFS(
                healerReachableTiles,
                unit.Grid,
                unit.transform.position,
                unit.Stats.MovementStat
            );

            foreach (var ally in turn.GetEnemyUnits().Where(a => a != unit))
            {
                float hpPercent = ally.Stats.CurrentHealthStat / (float)ally.Stats.MaxHealthStat;
                if (hpPercent >= 0.25f) continue;

                // verificamos si tenemos al menos una tile adyacente alcanzable
                if (GetAdjacentTilesPos(ally).Any(adj => healerReachableTiles.Contains(adj)))
                {
                    criticalAlliesPos.Add(ally.transform.position);
                }
            }
            return criticalAlliesPos.Count > 0;
        }

        public bool DetectWoundedAlliesInHealRange()
        {
            LazyInit();
            woundedAlliesPos.Clear();
            healerReachableHealPositions.Clear();

            var unit = baseAI.GetUnit();
            var turn = baseAI.GetTurnSystem();
            var grid = unit.Grid;

            grid.TryWorldToGridPosition(unit.transform.position, out Vector3 gridPos);
            AStarPathFinder.InsertMeleeAttackGridPositions(healerReachableHealPositions, grid, gridPos);

            foreach (var ally in turn.GetEnemyUnits().Where(a => a != unit))
            {
                float hpPercent = ally.Stats.CurrentHealthStat / (float)ally.Stats.MaxHealthStat;

                if (hpPercent < 0.90f)
                {
                    grid.TryWorldToGridPosition(ally.transform.position, out Vector3 allyGridPos);
                    if (healerReachableHealPositions.Contains(allyGridPos))
                        woundedAlliesPos.Add(ally.transform.position);
                }
            }
            return woundedAlliesPos.Count > 0;
        }

        public bool DetectWoundedAlliesFar()
        {
            LazyInit();
            woundedAlliesPos.Clear();
            healerReachableTiles.Clear();

            var unit = baseAI.GetUnit();
            var turn = baseAI.GetTurnSystem();

            _ = AStarPathFinder.InsertReachableGridPositionsAsyncBFS(
                healerReachableTiles,
                unit.Grid,
                unit.transform.position,
                unit.Stats.MovementStat
            );

            foreach (var ally in turn.GetEnemyUnits().Where(a => a != unit))
            {
                float hpPercent = ally.Stats.CurrentHealthStat / (float)ally.Stats.MaxHealthStat;
                if (hpPercent >= 0.90f) continue;

                // verificamos si tenemos al menos una tile adyacente alcanzable
                if (GetAdjacentTilesPos(ally).Any(adj => healerReachableTiles.Contains(adj)))
                {
                    woundedAlliesPos.Add(ally.transform.position);
                }
            }
            return woundedAlliesPos.Count > 0;
        }

        public bool DetectKillableEnemiesInAttackRange()
        {
            healerReachableAttackPositions.Clear();

            var unit = baseAI.GetUnit();
            var turn = baseAI.GetTurnSystem();

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


        //  TILE ADYACENTES

        private List<Vector3> GetAdjacentTilesPos(SimpleEntity ally)
        {
            LazyInit();

            var unit = baseAI.GetUnit();
            var grid = unit.Grid;

            var positions = new List<Vector3>();
            grid.TryWorldToGridPosition(ally.transform.position, out Vector3 gridPos);

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

        public void Heal()
        {
            Debug.Log("Healer is healing " + allyTarget.name);
            baseAI.GetEntityComponent<SpecialAbilityComponent>().DoAbility(allyTarget, false);
        }
        #endregion


        #region UtilitySystems
        
        public void SelectBestCriticalAlly() => SelectBestAlly(criticalAlliesPos);
        public void SelectBestWoundedAlly() => SelectBestAlly(woundedAlliesPos);

        public void SelectBestAlly(List<Vector3> allyTiles)
        {
            Debug.Log("hay estas posiciones de aliados: " + allyTiles.Count);
            LazyInit();

            var unit = baseAI.GetUnit();
            Unit best = null;
            float bestScore = float.MinValue;

            foreach (var pos in allyTiles)
            {
                Tile3d tile = unit.Grid.GetTileAtWorldPosition(pos);
                if (tile?.GetContainedEntity() is not Unit ally)
                    continue;

                float hpScore = 1f - (ally.Stats.CurrentHealthStat / (float)ally.Stats.MaxHealthStat);

                float typeScore = ally.Stats.SpecialAbilityType switch
                {
                    TankAbility _ => 1f,
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
                    best = ally;
                }
            }

            allyTarget = best;
            Debug.Log("Healer selected best ally: " + allyTarget.name);
        }

        public void SelectBestTileNearTargetAlly()
        {
            LazyInit();

            if (allyTarget == null) return;

            var unit = baseAI.GetUnit();

            healerReachableTiles.Clear();

            _ = AStarPathFinder.InsertReachableGridPositionsAsyncBFS(
                healerReachableTiles,
                unit.Grid,
                unit.transform.position,
                unit.Stats.MovementStat
            );

            var adjacentTiles = GetAdjacentTilesPos(allyTarget);
            var enemies = baseAI.GetTurnSystem().GetAllyUnits();

            Vector3 bestTile = Vector3.zero;
            float bestScore = float.MinValue;

            foreach (var t in adjacentTiles)
            {
                if (!healerReachableTiles.Contains(t)) continue;

                float avgDistToEnemies = enemies
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
                Debug.Log("Healer selected the best tile close to an ally: " + bestTile);
            }
        }

        public void SelectSafestTile()
        {
            LazyInit();

            var unit = baseAI.GetUnit();
            var turn = baseAI.GetTurnSystem();

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

                foreach (var ally in allies)
                {
                    if (!unit.Grid.TryWorldToGridPosition(ally.transform.position, out Vector3 allyGridPos))
                        continue;

                    float dist = Vector3.Distance(tile, allyGridPos);
                    score += Mathf.Clamp(10f / (dist + 1f), 0f, 10f) * 2f; // Cuanto más cerca, mejor
                }

                foreach (var enemy in enemies)
                {
                    if (!unit.Grid.TryWorldToGridPosition(enemy.transform.position, out Vector3 enemyGridPos))
                        continue;

                    float dist = Vector3.Distance(tile, enemyGridPos);
                    score += Mathf.Clamp(dist, 0f, 10f) * 0.5f; // Cuanto más lejos, mejor
                }

                int enemiesClose = enemies.Count(e =>
                {
                    unit.Grid.TryWorldToGridPosition(e.transform.position, out Vector3 pos);
                    return Vector3.Distance(tile, pos) < 3f;
                });

                float danger = enemiesClose * 5f;
                score -= danger;

                if (score > bestScore)
                {
                    bestScore = score;
                    bestTile = tile;
                }
            }

            if (bestScore == float.MinValue)
                return;

            baseAI.TargetedGridPosToMove = bestTile;

            if (unit.Grid.TryGridToWorldPosition(bestTile, out Vector3 targetWorld))
            {
                baseAI.TargetedPosToMove = targetWorld.CenterOnTile();
                Debug.Log("Healer selected safest tile: " + bestTile);
            }
        }

        #endregion
    }
}
