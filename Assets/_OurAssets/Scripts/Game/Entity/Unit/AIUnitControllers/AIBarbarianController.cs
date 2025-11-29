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
    public class AIBarbarianController : MonoBehaviour
    {
        List<Vector3> barbarianReachableTiles = new();
        List<Vector3> barbarianReachableAttackTiles = new();
        List<SimpleEntity> allEntities = new List<SimpleEntity>();

        AIUnitController baseAI;

        SimpleEntity target;

        //garantiza que el baseAI esté listo
        private void LazyInit()
        {
            if (baseAI != null) return;

            baseAI = GetComponent<AIUnitController>();
            if (baseAI == null)
                Debug.LogError("AIBarbarianController NO encontró AIUnitController en el mismo GameObject.");
        }


        #region Perceptions

        public bool IsEntityClose()
        {
            LazyInit();

            barbarianReachableAttackTiles.Clear();
            allEntities.Clear();
            target = null;

            var unit = baseAI.GetUnit();
            var grid = unit.Grid;

            grid.TryWorldToGridPosition(unit.transform.position, out Vector3 startGridPos);

            barbarianReachableAttackTiles = baseAI.GetAdjacentTiles(unit);

            allEntities.AddRange(baseAI.GetTurnSystem().GetAllyUnits().Where(e => e != null));
            allEntities.AddRange(baseAI.GetTurnSystem().GetWallEntities().Where(e => e != null));

            List<SimpleEntity> enemiesInRange = new();
            List<SimpleEntity> neutralsInRange = new();

            foreach (var ent in allEntities)
            {
                grid.TryWorldToGridPosition(ent.transform.position, out Vector3 entGridPos);

                bool inRange = barbarianReachableAttackTiles
                    .Any(t => Vector3.Distance(t, entGridPos) < 0.01f);

                if (!inRange)
                    continue;

                if (ent.GetSide() == BattleSide.Ally)
                    enemiesInRange.Add(ent);
                else if (ent.GetSide() == BattleSide.Neutral)
                    neutralsInRange.Add(ent);
            }

            if (enemiesInRange.Count > 0)
            {
                target = enemiesInRange
                    .OrderBy(e => ((Unit)e).Stats.CurrentHealthStat)
                    .First();

                return true;
            }

            if (neutralsInRange.Count > 0)
            {
                target = neutralsInRange[0];
                return true;
            }

            return false;
        }

        public bool IsEntityInMovementRange()
        {
            LazyInit();

            target = null;
            allEntities.Clear();
            barbarianReachableTiles.Clear();

            var unit = baseAI.GetUnit();
            var grid = unit.Grid;

            _ = AStarPathFinder.InsertReachableGridPositionsAsyncBFS(
                barbarianReachableTiles,
                grid,
                unit.GetSide(),
                unit.transform.position,
                unit.Stats.MovementStat
            );

            allEntities.AddRange(baseAI.GetTurnSystem().GetAllyUnits().Where(e => e != null));
            allEntities.AddRange(baseAI.GetTurnSystem().GetWallEntities().Where(e => e != null));

            SimpleEntity closest = null;
            float closestDist = float.MaxValue;

            foreach (var ent in allEntities)
            {
                grid.TryWorldToGridPosition(ent.transform.position, out Vector3 entGridPos);

                List<Vector3> attackPositions = baseAI.GetAdjacentTilesToMove(ent);

                if (!attackPositions.Any(p => barbarianReachableTiles.Contains(p)))
                    continue;

                float dist = Vector3.Distance(unit.transform.position, ent.transform.position);

                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = ent;
                }
            }

            if (closest == null)
                return false;

            target = closest;
            return true;
        }


        public bool IsNeutralEntity()
        {
            Debug.Log($"VAMOS A VER QUE UNIDAD HA ELEGIDO MI COLEGA: {target.name}");

            if (target.GetSide() == BattleSide.Neutral)
            {
                return true;
            }
            else
            {
                baseAI.TargetedEnemy = target;
                Debug.Log("LA UNIDAD ELEGIDA NO ES NEUTRAL");
                return false;
            }

        }

        #endregion

        #region ActionLogic

        public void BreakWall()
        {
            Debug.Log("Barbarian is Breaking a Wall");
            baseAI.GetEntityComponent<SpecialAbilityComponent>().DoAbility(target, false);
        }

        #endregion

        #region UtilitySystems

        public void SelectBestTileNearEntity()
        {
            LazyInit();

            if (target == null) return;

            var unit = baseAI.GetUnit();
            barbarianReachableTiles.Clear();

            _ = AStarPathFinder.InsertReachableGridPositionsAsyncBFS(
                barbarianReachableTiles,
                unit.Grid,
                unit.GetSide(),
                unit.transform.position,
                unit.Stats.MovementStat
            );

            var adjacentTiles = baseAI.GetAdjacentTilesToMove(target);
            var allEnemies = baseAI.GetTurnSystem().GetAllyUnits();

            Vector3 bestTile = Vector3.zero;
            float bestScore = float.MaxValue;

            foreach (var t in adjacentTiles)
            {
                if (!barbarianReachableTiles.Contains(t)) continue;

                float score = Vector3.Distance(t, unit.transform.position);

                if (score < bestScore)
                {
                    bestScore = score;
                    bestTile = t;
                }
            }

            if (bestScore == float.MaxValue)
                return;

            baseAI.TargetedGridPosToMove = bestTile;

            if (unit.Grid.TryGridToWorldPosition(bestTile, out Vector3 targetWorld))
            {
                baseAI.TargetedPosToMove = targetWorld.CenterOnTile();
                Debug.Log("Barbarian selected best tile near enemy: " + bestTile);
            }
        }

        #endregion
    }
}
