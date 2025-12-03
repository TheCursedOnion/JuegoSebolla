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
    public class AIRobBossController : MonoBehaviour
    {

        List<Vector3> robReachableTiles = new();

        AIUnitController baseAI;

        SimpleEntity enemyTarget;

        //garantiza que el baseAI esté listo
        private void LazyInit()
        {
            if (baseAI != null) return;

            baseAI = GetComponent<AIUnitController>();
            if (baseAI == null)
                Debug.LogError("AIRobBossController NO encontró AIUnitController en el mismo GameObject.");
        }


        #region Perceptions

        public bool CanExplode()
        {
            LazyInit();

            return true;
        }
        #endregion

        #region ActionLogic

        public void Explode()
        {
            /*foreach (var ally in alliesToBuff)
            {
                Debug.Log("BossRob is EXPLODING: " + ally.name);
                baseAI.GetEntityComponent<SpecialAbilityComponent>().DoAbility(ally, false);
            }*/
            
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

                Debug.Log($"Evaluando enemigo {enemy.name}: HP Score = {hpScore}");

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
            Debug.Log("RobBoss selected best enemy to attack: " + enemyTarget?.name);
        }

        public void SelectBestTileNearEnemy()
        {
            LazyInit();

            if (enemyTarget == null) return;

            var unit = baseAI.GetUnit();
            robReachableTiles.Clear();

            _ = AStarPathFinder.InsertReachableGridPositionsAsyncBFS(
                robReachableTiles,
                unit.Grid,
                unit.GetSide(),
                unit.transform.position,
                unit.Stats.MovementStat
            );

            var adjacentTiles = baseAI.GetAdjacentTilesToMove(enemyTarget);
            var allEnemies = baseAI.GetTurnSystem().GetAllyUnits();

            Vector3 bestTile = Vector3.zero;
            float bestScore = float.MinValue;

            foreach (var t in adjacentTiles)
            {
                if (!robReachableTiles.Contains(t)) continue;

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
                Debug.Log("RobBoss selected best tile near enemy: " + bestTile);
            }
        } 

        #endregion
    }
}
