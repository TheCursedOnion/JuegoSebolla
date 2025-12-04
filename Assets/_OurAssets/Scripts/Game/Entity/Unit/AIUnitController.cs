using BehaviourAPI.UnityToolkit.GUIDesigner.Runtime;
using CursedOnion.Extensions;
using CursedOnion.Game.Entity.Components;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.Game.Systems.Level;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

namespace CursedOnion.Game.Entity
{
    public class AIUnitController : EntityComponentController
    {
        Unit unit;
        public Unit GetUnit() => unit;

        TurnSystem turnSystem;
        public TurnSystem GetTurnSystem() => turnSystem;

        public Vector3 TargetedGridPosToMove;
        public Vector3 TargetedPosToMove;
        public SimpleEntity TargetedEnemy;

        public List<Vector3> reachableAttackPositions = new();
        public List<Vector3> reachableMovePositions = new();
        public List<Vector3> enemyPositions = new();

        public override void Initialize(SimpleEntity entity, EntityComponents components)
        {
            base.Initialize(entity, components);
            unit = entity as Unit;
            
            turnSystem = entity.LevelManager.GetTurnSystem();
            
            entity.LevelEvents.OnPathNotFound += CancelMove;
        }
        public override void ProcessTurn()
        {
            AssignedEntity.HasTurn = true;

            if (AssignedEntity.StatusHandler.HasConfusionEffect()) 
            {
                EndAITurn();
                AssignedEntity.UpdateStatusEffects();
                return;
            }

            base.ProcessTurn();
            
        }

        protected override void EndTurn()
        {
            base.EndTurn();
            AssignedEntity.ActionHandler.ResetFlag(ActionFlag.CannotCounter);
        }
        public bool HasTurn() 
        {
            bool hasTurn = AssignedEntity != null && AssignedEntity.HasTurn;
            return hasTurn;
        } 

        #region Percepciones Generales

        public bool EnemyInAttackRange()
        {
            enemyPositions.Clear();
            reachableAttackPositions.Clear();

            var grid = unit.Grid;
            unit.Grid.TryWorldToGridPosition(unit.transform.position, out Vector3 startGridPos);

            if (unit.Stats.SpecialAbilityType is ArcherAbility archer)
            {
                AStarPathFinder.InsertRangedAttackGridPositions(reachableAttackPositions, unit.Grid, startGridPos, 2);
            }
            else
            {
                reachableAttackPositions = GetAdjacentTiles(unit);
            }

            foreach (var enemy in turnSystem.GetAllyUnits())
            {
                grid.TryWorldToGridPosition(enemy.transform.position, out Vector3 enemyGridPos);

                if (reachableAttackPositions.Any(p => Vector3.Distance(p, enemyGridPos) < 0.01f))
                {
                    enemyPositions.Add(enemy.transform.position);
                }
            }
            return enemyPositions.Count > 0;
        }

        public bool EnemyInMovementRange()
        {
            enemyPositions.Clear();
            reachableMovePositions.Clear();

            _ = AStarPathFinder.InsertReachableGridPositionsAsyncBFS(
                reachableMovePositions,
                unit.Grid,
                unit.GetSide(),
                unit.transform.position,
                unit.Stats.MovementStat
            );
       
            foreach (var enemy in turnSystem.GetAllyUnits())
            {
                unit.Grid.TryWorldToGridPosition(enemy.transform.position, out Vector3 enemyGridPos);
                List<Vector3> attackPositions;

                if (unit.Stats.SpecialAbilityType is ArcherAbility archer)
                {
                    attackPositions = new List<Vector3>();
                    AStarPathFinder.InsertRangedAttackGridPositions(attackPositions, unit.Grid, enemyGridPos, 2);
                }
                else
                {
                    attackPositions = GetAdjacentTilesToMove(enemy);
                }

                if (attackPositions.Any(adj => reachableMovePositions.Contains(adj)))
                {
                    enemyPositions.Add(enemy.transform.position);
                }
            }
            return enemyPositions.Count > 0;
        }

        #endregion

        #region Acciones Generales

        bool doingMove = false;
        void CancelMove() => doingMove = false;
        public void EnemyMove()
        {
            Debug.Log($"EL ENEMIGO {gameObject.name} VA A MOVERSE A " + TargetedPosToMove);
            doingMove = true;
            GetEntityComponent<MoveEntityComponent>().DoMove(TargetedGridPosToMove, false);
        }
        
        public BehaviourAPI.Core.Status EndMove()
        {
            Vector3 pos = unit.transform.position;
            Vector3 target = TargetedPosToMove;
            
            bool closeEnough = Vector3.Distance(pos, target) < 0.65f;
            if (!closeEnough && doingMove)
                return BehaviourAPI.Core.Status.Running;

            Debug.Log($"EL ENEMIGO SE HA MOVIDO A " + TargetedPosToMove + "| ESTÁ EN POSICION: " + pos);
            TargetedGridPosToMove = Vector3.zero;
            TargetedPosToMove = Vector3.zero;
            return BehaviourAPI.Core.Status.Success;
        }

        public void EnemyAttack()
        {
            Debug.Log("EL ENEMIGO VA A ATACAR A" + TargetedEnemy);
            GetEntityComponent<AttackEntityComponent>().DoAttack(TargetedEnemy, false);
        }

        public BehaviourAPI.Core.Status EndAction() => BehaviourAPI.Core.Status.Success;

        #endregion

        #region Helpers

        public bool SearchAndFindPath()
        {
            Debug.Log("EL ENEMIGO VA A BUSCAR UNA UNIDAD Y MOVERSE HACIA ELLA");

            TargetedGridPosToMove = Vector3.zero;

            Unit closestAlly = null;
            float bestAllyDistance = float.MaxValue;

            foreach (var ally in turnSystem.GetAllyUnits())
            {
                float dist = Vector3.Distance(unit.transform.position, ally.transform.position);
                if (dist < bestAllyDistance)
                {
                    bestAllyDistance = dist;
                    closestAlly = ally;
                }
            }

            if (closestAlly == null)
                return false;

            float maxAllowedDistance = unit.Stats.MovementStat * 2; // demasiado lejos, no te busca

            if (bestAllyDistance > maxAllowedDistance)
            {
                return false;
            }

            var adjacentTiles = GetAdjacentTilesToMove(closestAlly);
            if (adjacentTiles.Count == 0)
                return false;

            List<Vector3> bestPath = null;
            Vector3 bestTargetTile = default;
            float bestPathLen = float.MaxValue;

            if (!unit.Grid.TryWorldToGridPosition(unit.transform.position, out Vector3 startGridPos))
                return false;

            foreach (var tile in adjacentTiles)
            {
                var path = AStarPathFinder.FindPath(startGridPos, tile, unit.Grid, AssignedEntity.GetSide());
                if (path == null || path.Count == 0)
                {
                    Debug.Log("No se encontró camino hacia el tile " + tile);
                    continue;
                }
                
                if (path.Count < bestPathLen)
                {
                    bestPathLen = path.Count;
                    bestPath = path;
                    bestTargetTile = tile;
                }
            }

            if (bestPath == null)
                return false;

            reachableMovePositions.Clear();
            _ = AStarPathFinder.InsertReachableGridPositionsAsyncBFS(
                reachableMovePositions,
                unit.Grid,
                unit.GetSide(),
                unit.transform.position,
                unit.Stats.MovementStat
            );

            if (reachableMovePositions.Count == 0)
                return false;

            Vector3 chosenTile = Vector3.zero;

            for (int i = bestPath.Count - 1; i >= 0; i--)
            {
                if (!unit.Grid.TryWorldToGridPosition(bestPath[i], out Vector3 candidate))
                    return false;

                if (reachableMovePositions.Contains(candidate))
                {
                    chosenTile = candidate;
                    break;
                }
            }

            TargetedGridPosToMove = chosenTile;

            if (!unit.Grid.TryGridToWorldPosition(chosenTile, out Vector3 targetWorld))
                return false;

            TargetedPosToMove = targetWorld.CenterOnTile();

            return true;
        }

        //  TILE ADYACENTES
        public List<Vector3> GetAdjacentTilesToMove(SimpleEntity entity)
        {
            var grid = unit.Grid;

            var positions = new List<Vector3>();
            grid.TryWorldToGridPosition(entity.transform.position, out Vector3 gridPos);

            Tile3d currentTile = grid.GetTileAtGridPosition(gridPos);

            // entity en escalera

            if (currentTile.IsStairTile())
            {
                List<Vector3> exits = currentTile.GetExitDirectionVector();

                foreach (var exit in exits)
                {
                    Vector3 pos = gridPos + exit;

                    if (!grid.IsGridPositionInBounds(pos))
                        continue;

                    Tile3d t = grid.GetTileAtGridPosition(pos);

                    if (t.IsEmptyTile() && t.GetContainedEntity() == null)
                        positions.Add(pos);
                }

                return positions;
            }

            // no esta en escalera

            Vector3[] dirs =
            {
                new Vector3( 1,0,0),
                new Vector3(-1,0,0),
                new Vector3(0,0, 1),
                new Vector3(0,0,-1),
            };

            foreach (var d in dirs)
            {
                Vector3 pos = gridPos + d;

                if (!grid.IsGridPositionInBounds(pos))
                    continue;

                Tile3d tile = grid.GetTileAtGridPosition(pos);

                // comprobar si abajo hay escalera

                if (tile.IsEmptyTile())
                {
                    Vector3 belowPos = pos + new Vector3(0, -1, 0);

                    if (grid.IsGridPositionInBounds(belowPos))
                    {
                        Tile3d belowTile = grid.GetTileAtGridPosition(belowPos);

                        if (belowTile.IsStairTile())
                        {
                            if (belowTile.GetContainedEntity() == null)
                                positions.Add(belowPos);

                            continue;
                        }
                    }

                    // si NO hay escalera debajo
                    if (tile.GetContainedEntity() == null)
                    {
                        positions.Add(pos);
                    }

                    continue;
                }

                //tile normal y corriente

                if (!(tile.IsStairTile()) &&
                    tile.IsEmptyTile() &&
                    tile.GetContainedEntity() == null)
                {
                    positions.Add(pos);
                }
            }

            return positions;
        }

        public List<Vector3> GetAdjacentTiles(SimpleEntity entity)
        {
            var grid = unit.Grid;

            var positions = new List<Vector3>();
            grid.TryWorldToGridPosition(entity.transform.position, out Vector3 gridPos);

            Tile3d currentTile = grid.GetTileAtGridPosition(gridPos);

            // entity en escalera

            if (currentTile.IsStairTile())
            {
                List<Vector3> exits = currentTile.GetExitDirectionVector();

                foreach (var exit in exits)
                {
                    Vector3 pos = gridPos + exit;

                    if (!grid.IsGridPositionInBounds(pos))
                        continue;

                    positions.Add(pos);
                }

                return positions;
            }

            // no esta en escalera

            Vector3[] dirs =
            {
                new Vector3( 1,0,0),
                new Vector3(-1,0,0),
                new Vector3(0,0, 1),
                new Vector3(0,0,-1),
            };

            foreach (var d in dirs)
            {
                Vector3 pos = gridPos + d;

                if (!grid.IsGridPositionInBounds(pos))
                    continue;

                Tile3d tile = grid.GetTileAtGridPosition(pos);

                // comprobar si abajo hay escalera

                if (tile.IsEmptyTile())
                {
                    Vector3 belowPos = pos + new Vector3(0, -1, 0);

                    if (grid.IsGridPositionInBounds(belowPos))
                    {
                        Tile3d belowTile = grid.GetTileAtGridPosition(belowPos);

                        if (belowTile.IsStairTile())
                        {
                            positions.Add(belowPos);
                            continue;
                        }
                    }
                    positions.Add(pos);
                    continue;
                }

                //tile normal y corriente

                if (!(tile.IsStairTile()) && tile.IsEmptyTile())
                {
                    positions.Add(pos);
                }
            }

            return positions;
        }
        #endregion

        public void EndAITurn()
        {
            if (turnSystem != null &&  AssignedEntity.HasTurn)
            {
                AssignedEntity.HasTurn = false;
                turnSystem.EndTurnForAIUnit(unit);
            }
        }
        
        public override void Dispose()
        {
            base.Dispose();
            if (AssignedEntity != null)
            {
                AssignedEntity.LevelEvents.OnPathNotFound -= CancelMove;
            }
        }
    }
}
