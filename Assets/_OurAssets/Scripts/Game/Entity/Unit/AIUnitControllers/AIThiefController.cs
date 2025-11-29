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
    public class AIThiefController : MonoBehaviour
    {
        List<Vector3> thiefReachableTiles = new();
        List<Vector3> thiefReachableAttackTiles = new();

        AIUnitController baseAI;

        SimpleEntity enemyTarget;

        //garantiza que el baseAI esté listo
        private void LazyInit()
        {
            if (baseAI != null) return;

            baseAI = GetComponent<AIUnitController>();
            if (baseAI == null)
                Debug.LogError("AIThiefController NO encontró AIUnitController en el mismo GameObject.");
        }


        #region Perceptions

       public bool CanConfuse()
        {
            if (enemyTarget == null)
                return false;

            if (enemyTarget.StatusHandler.IsConfused)
                return false;

            var unit = baseAI.GetUnit();
            var allies = baseAI.GetTurnSystem().GetEnemyUnits();

            bool anyNonThiefAllyClose = allies.Any(
                a => a != unit &&                              
                a.Stats.SpecialAbilityType is not ThiefAbility &&   
                Vector3.Distance(unit.transform.position, a.transform.position) <= 4f 
            );

            return anyNonThiefAllyClose;

        }

        #endregion

        #region ActionLogic

        public void Confuse()
        {
            Debug.Log("Thief is applying Confusion");
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
                    ArcherAbility _ => 1f,
                    HealerAbility _ => 0.9f,
                    ExplorerAbility _ => 0.8f,
                    ThiefAbility _ => 0.7f,
                    SoldierAbility _ => 0.7f,
                    BarbarianAbility _ => 0.6f,
                    TankAbility _ => 0.4f,
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
            Debug.Log("Thief selected best enemy to attack: " + enemyTarget?.name);
        }

        public void SelectBestTileNearEnemy()
        {
            LazyInit();

            if (enemyTarget == null) return;

            var unit = baseAI.GetUnit();
            thiefReachableTiles.Clear();

            _ = AStarPathFinder.InsertReachableGridPositionsAsyncBFS(
                thiefReachableTiles,
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
                if (!thiefReachableTiles.Contains(t)) continue;

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

        #endregion
    }
}
