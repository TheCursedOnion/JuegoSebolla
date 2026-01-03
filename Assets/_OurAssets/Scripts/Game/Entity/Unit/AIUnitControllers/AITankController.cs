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
    public class AITankController : MonoBehaviour
    {
        List<Vector3> tankReachableTiles = new();
        List<Vector3> enemiesPositionsClose = new();

        AIUnitController baseAI;

        SimpleEntity enemyTarget;

        //garantiza que el baseAI esté listo
        private void LazyInit()
        {
            if (baseAI != null) return;

            baseAI = GetComponent<AIUnitController>();
            if (baseAI == null)
                Debug.LogError("AITankController NO encontró AIUnitController en el mismo GameObject.");
        }


        #region Perceptions

        public bool CanFortify()
        {
            LazyInit();
            var unit = baseAI.GetUnit();
            var enemies = baseAI.GetTurnSystem().GetAllyUnits();
            enemiesPositionsClose.Clear();

            foreach (var enemy in enemies)
            {
                float dist = Vector3.Distance(unit.transform.position, enemy.transform.position);
                if (dist < 2)
                {
                    enemiesPositionsClose.Add(enemy.transform.position);
                }
            }

            if (enemiesPositionsClose.Count > 2)
                return true;

            return false;
        }

        #endregion

        #region ActionLogic

        public void Fortify()
        {
            Debug.Log("Tank is Fortifying");
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

                float threatScore = enemy.Stats.SpecialAbilityType switch
                {
                    ArcherAbility => 1f,   
                    ThiefAbility => 1f,  
                    SoldierAbility => 0.8f,
                    BarbarianAbility => 0.7f, 
                    HealerAbility => 0.6f, 
                    ExplorerAbility => 0.4f,
                    TankAbility => 0.2f, 
                    _ => 0.5f
                };

                float lowHpScore = 1f - (enemy.Stats.CurrentHealthStat / (float)enemy.Stats.MaxHealthStat);

                float dangerToAlliesScore = 0f;

                foreach (var ally in baseAI.GetTurnSystem().GetEnemyUnits())
                {
                    float dist = Vector3.Distance(enemy.transform.position, ally.transform.position);
                    if (dist < 2f) 
                        dangerToAlliesScore += 0.5f;
                }

                float score = threatScore * 1.2f + lowHpScore * 0.6f + dangerToAlliesScore;

                if (score > bestScore)
                {
                    bestScore = score;
                    best = enemy;
                }
            }

            enemyTarget = best;
            baseAI.TargetedEnemy = enemyTarget;
            Debug.Log("Tank selected best enemy to attack: " + enemyTarget?.name);
        }

        public void SelectBestTileNearEnemy()
        {
            LazyInit();

            if (enemyTarget == null) return;

            var unit = baseAI.GetUnit();
            tankReachableTiles.Clear();

            _ = AStarPathFinder.InsertReachableGridPositionsAsyncBFS(
                tankReachableTiles,
                unit.Grid,
                unit.GetSide(),
                unit.transform.position,
                unit.Stats.MovementStat
            );

            var adjacentTiles = baseAI.GetAdjacentTilesToMove(enemyTarget);
            var allEnemies = baseAI.GetTurnSystem().GetAllyUnits();
            var allAllies = baseAI.GetTurnSystem().GetEnemyUnits();

            Vector3 bestTile = Vector3.zero;
            float bestScore = float.MinValue;

            foreach (var t in adjacentTiles)
            {
                if (!tankReachableTiles.Contains(t)) continue;

                float enemiesNearby = allEnemies.Count(e => Vector3.Distance(t, e.transform.position) <= 2.5f);

                float avgDistToAllies = allAllies.Select(a => Vector3.Distance(t, a.transform.position)).DefaultIfEmpty(3f).Average();
                float allyProximityScore = Mathf.Clamp(3f - avgDistToAllies, 0f, 3f);

                float score = enemiesNearby * 1.5f + allyProximityScore;

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
                Debug.Log("Tank selected best tile near enemy: " + bestTile);
            }
        }

        #endregion
    }
}
