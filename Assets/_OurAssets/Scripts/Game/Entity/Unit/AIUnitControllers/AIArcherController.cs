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
    public class AIArcherController : MonoBehaviour
    {

        List<Vector3> archerReachableTiles = new();
        List<Vector3> archerReachableAttackTiles = new();

        AIUnitController baseAI;

        SimpleEntity enemyTarget;

        //garantiza que el baseAI esté listo
        private void LazyInit()
        {
            if (baseAI != null) return;

            baseAI = GetComponent<AIUnitController>();
            if (baseAI == null)
                Debug.LogError("AIArcherController NO encontró AIUnitController en el mismo GameObject.");
        }


        #region Perceptions 

        public bool CanTripleShot()
        {
            LazyInit();
            var unit = baseAI.GetUnit();

            if (unit == null || enemyTarget == null)
                return false;

            var grid = unit.Grid;
            if (!grid.TryWorldToGridPosition(unit.transform.position, out Vector3 unitGridPos) ||
                !grid.TryWorldToGridPosition(enemyTarget.transform.position, out Vector3 targetGridPos))
                return false;

            Vector3 direction = targetGridPos - unitGridPos;
            direction.y = 0;

            if (Mathf.Abs(direction.x) > 0 && Mathf.Abs(direction.z) > 0)
                return false;

            direction = new Vector3(
                Mathf.Clamp(direction.x, -1, 1),
                0,
                Mathf.Clamp(direction.z, -1, 1)
            );

            int consecutiveEnemies = 0;

            for (int i = 0; i < 3; i++)
            {
                Vector3 posToCheck = targetGridPos + direction * i;

                Tile3d tile = grid.GetTileAtGridPosition(posToCheck);
                if (tile == null)
                    break;

                SimpleEntity entity = tile.GetContainedEntity();
                if (entity is Unit enemyUnit && enemyUnit.GetSide() != unit.GetSide())
                {
                    consecutiveEnemies++;
                }
                else
                {
                    break; 
                }
            }

            return consecutiveEnemies >= 2;
        }
        #endregion

        #region ActionLogic

        public void TripleShot()
        {
            Debug.Log("Archer is using TripleShot");
            baseAI.GetEntityComponent<SpecialAbilityComponent>().DoAbility(enemyTarget, false);
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

                float score = hpScore + typeScore * 1.2f;

                if (score > bestScore)
                {
                    bestScore = score;
                    best = enemy;
                }
            }

            enemyTarget = best;
            baseAI.TargetedEnemy = enemyTarget;
            Debug.Log("Archer selected best enemy to attack: " + enemyTarget?.name);
        }

        public void SelectBestTileNearEnemy()
        {
            LazyInit();

            if (enemyTarget == null) return;

            var unit = baseAI.GetUnit();
            var grid = unit.Grid;
            archerReachableTiles.Clear();

            _ = AStarPathFinder.InsertReachableGridPositionsAsyncBFS(
                archerReachableTiles,
                unit.Grid,
                unit.GetSide(),
                unit.transform.position,
                unit.Stats.MovementStat
            );

            List<Vector3> attackPositions = new List<Vector3>();

            grid.TryWorldToGridPosition(enemyTarget.transform.position, out Vector3 enemyGridPos);

            AStarPathFinder.InsertManhattanAttackGridPositions(attackPositions, unit.Grid, enemyGridPos, 2, false);

            var candidateTiles = archerReachableTiles
                .Intersect(attackPositions)
                .ToList();



            Debug.Log("Archer found candidate tiles near enemy: " + candidateTiles.Count);

            Vector3 bestTile = candidateTiles[0];
            float bestScore = float.MinValue;

            var allEnemies = baseAI.GetTurnSystem().GetAllyUnits();

            foreach (var t in candidateTiles)
            {
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
                Debug.Log("Archer selected best tile near enemy: " + bestTile);
            }
        }

        #endregion
    }
}
