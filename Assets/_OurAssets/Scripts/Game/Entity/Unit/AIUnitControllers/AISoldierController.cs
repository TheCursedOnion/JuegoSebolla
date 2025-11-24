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

        public List<Vector3> posCloseToHealers = new();

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

        public bool WeakAndHealerClose()
        {
            LazyInit();

            float healthPercent = AssignedEntity.Stats.CurrentHealthStat / AssignedEntity.Stats.MaxHealthStat;

            if(healthPercent>=0.5f)
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

        public void Rage()
        {
            Debug.Log("Soldier raging!");
        }
        #endregion


        #region UtilitySystems

        public void SelectBestTileNearEnemy()
        {
            //
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

                foreach (var enemy in baseAI.GetTurnSystem().GetEnemyUnits())
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
