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
    public class AIPersianBossController : MonoBehaviour
    {

        List<Vector3> persaReachableTiles = new();
        List<SimpleEntity> alliesToBuff = new();

        AIUnitController baseAI;

        SimpleEntity enemyTarget;

        //garantiza que el baseAI esté listo
        private void LazyInit()
        {
            if (baseAI != null) return;

            baseAI = GetComponent<AIUnitController>();
            if (baseAI == null)
                Debug.LogError("AIPersianBossController NO encontró AIUnitController en el mismo GameObject.");
        }


        #region Perceptions

        public bool CanRageGroup()
        {
            LazyInit();

            alliesToBuff.Clear();

            var unit = baseAI.GetUnit();
            var allies = baseAI.GetTurnSystem().GetEnemyUnits();
            var enemies = baseAI.GetTurnSystem().GetAllyUnits();

            int nearbyAllies = 0;
            int nearbyEnemies = 0;

            foreach (var ally in allies)
            {
                if (ally == unit) continue;

                float dist = Vector3.Distance(unit.transform.position, ally.transform.position);
                if (dist <= 4)
                {
                    alliesToBuff.Add(ally);
                    nearbyAllies++;
                }
            }

            foreach (var enemy in enemies)
            {
                float dist = Vector3.Distance(unit.transform.position, enemy.transform.position);
                if (dist <= 4)
                {
                    nearbyEnemies++;
                }
            }

            bool condition =  nearbyAllies >= 2 && nearbyEnemies >= 1;

            if (!condition)
                return false;

            return nearbyAllies+1 > nearbyEnemies;
        }
        #endregion

        #region ActionLogic

        public void RageGroup()
        {
            foreach (var ally in alliesToBuff)
            {
                Debug.Log("BossPersa is buffing: " + ally.name);
                baseAI.GetEntityComponent<SpecialAbilityComponent>().DoAbility(ally, false);
            }
            
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
            Debug.Log("PersianBoss selected best enemy to attack: " + enemyTarget?.name);
        }

        public void SelectBestTileNearEnemy()
        {
            LazyInit();

            if (enemyTarget == null) return;

            var unit = baseAI.GetUnit();
            persaReachableTiles.Clear();

            _ = AStarPathFinder.InsertReachableGridPositionsAsyncBFS(
                persaReachableTiles,
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
                if (!persaReachableTiles.Contains(t)) continue;

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
                Debug.Log("PersianBoss selected best tile near enemy: " + bestTile);
            }
        } 

        #endregion
    }
}
