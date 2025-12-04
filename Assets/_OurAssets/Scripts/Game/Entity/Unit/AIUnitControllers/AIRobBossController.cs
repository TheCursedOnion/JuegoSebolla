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
        List<Vector3> areaTiles = new();
        List<SimpleEntity> bestAffectedList = new();

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

        public bool CanFindBetterSkillPosition()
        {
            LazyInit();

            var unit = baseAI.GetUnit();
            var grid = unit.Grid;

            _ = AStarPathFinder.InsertReachableGridPositionsAsyncBFS(
                robReachableTiles, 
                grid, 
                unit.GetSide(), 
                unit.transform.position, 
                unit.Stats.MovementStat
            );

            grid.TryWorldToGridPosition(unit.transform.position, out Vector3 currentGridPos);
            robReachableTiles.Add(currentGridPos);

            int bestEnemies = 0;
            int bestAllies = 999;
            Vector3 bestTile = currentGridPos;

            bool foundBetter = false;

            foreach (var tile in robReachableTiles)
            {
                EvaluateAOEManhattan(tile, unit, out int enemies, out int allies, out List<SimpleEntity> affected);

                if (allies >= enemies) continue;

                bool better =
                    enemies > bestEnemies ||
                    (enemies == bestEnemies && allies < bestAllies);

                if (better)
                {
                    bestEnemies = enemies;
                    bestAllies = allies;
                    bestTile = tile;
                    bestAffectedList = affected;
                    foundBetter = true;
                }
            }

            if (!foundBetter || bestTile == currentGridPos)
                return false;

            baseAI.TargetedGridPosToMove = bestTile;

            if (unit.Grid.TryGridToWorldPosition(bestTile, out Vector3 targetWorld))
            {
                baseAI.TargetedPosToMove = targetWorld.CenterOnTile();
            }

            return true;
        }

        private void EvaluateAOEManhattan(Vector3 tile, Unit caster, out int enemies, out int allies, out List<SimpleEntity> affectedUnits)
        {
            enemies = 0;
            allies = 0;

            var grid = caster.Grid;
            

            affectedUnits = new List<SimpleEntity>();
            areaTiles.Clear();
            AStarPathFinder.InsertManhattanAttackGridPositions(areaTiles, grid, tile, 2, true);

            var ts = baseAI.GetTurnSystem();

            foreach (var pos in areaTiles)
            {
                Tile3d t = grid.GetTileAtGridPosition(pos);
                if (t == null) continue;

                if (t.GetContainedEntity() is SimpleEntity e && e != caster && e != null)
                {
                    if (!affectedUnits.Contains(e))
                        affectedUnits.Add(e);

                    if (ts.GetAllyUnits().Contains(e)) enemies++;
                    else if (ts.GetEnemyUnits().Contains(e)) allies++;
                }
            }
        }

        public bool ShouldUseSkillOnSpot()
        {
            LazyInit();

            var unit = baseAI.GetUnit();
            var grid = unit.Grid;

            grid.TryWorldToGridPosition(unit.transform.position, out Vector3 startGrid);

            EvaluateAOEManhattan(startGrid, unit, out int enemies, out int allies, out List<SimpleEntity> affected);

            if (enemies > allies)
            {
                bestAffectedList = affected; 
                return true;
            }

            return false;
        }


        #endregion

        #region ActionLogic

        public void Explode()
        {
            foreach (var unit in bestAffectedList.ToList())
            {
                if (unit == null) continue;
                Debug.Log("BossRob is EXPLODING damaging: " + unit.name);
                baseAI.GetEntityComponent<SpecialAbilityComponent>().DoAbility(unit, false);
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

                float score = hpScore;

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

        #endregion
    }
}
