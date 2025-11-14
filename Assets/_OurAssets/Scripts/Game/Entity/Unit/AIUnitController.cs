using BehaviourAPI.Core;
using BehaviourAPI.UnityToolkit.GUIDesigner.Runtime;
using CursedOnion.Extensions;
using CursedOnion.Game.Entity.UI;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.Game.Systems.Level;
using JetBrains.Annotations;
using Reflex.Attributes;
using System.Collections.Generic;
using CursedOnion.Game.Entity.Components;
using UnityEngine;

namespace CursedOnion.Game.Entity
{
    public class AIUnitController : EntityComponentController
    {
        LevelManager levelManager;
        AssetBehaviourRunner runner;
        bool startTurn;

        public AssetBehaviourRunner GetBehaviourRunner() => runner;

        TurnSystem turnSystem;

        List<Unit> allyUnit;

        Unit unit;

        List<Vector3> reachableAttackPositions = new();
        List<Vector3> reachableMovePositions = new();
        Vector3 targetedGridPosToMove;
        Vector3 targetedPosToMove;
        SimpleEntity targetedEnemy;

        public override void Initialize(SimpleEntity entity, EntityComponents components)
        {
            base.Initialize(entity, components);
            runner = gameObject.GetComponent<AssetBehaviourRunner>();
            turnSystem = entity.LevelManager.GetTurnSystem();
            unit = entity as Unit;
        }
        public override void ProcessTurn()
        {
            startTurn = true;
        }

        public bool StartTurn()
        {
            return startTurn;
        }

        public bool IsEnemyInAttackRange()
        {
            var grid = unit.Grid;
            var position = unit.transform.position;

            reachableAttackPositions.Clear();
            bool isMeleeUnit = unit.Stats.SpecialAbilityType is not ArcherAbility;

            grid.TryWorldToGridPosition(position, out Vector3 gridPos);
            if (!isMeleeUnit)
            {
                AStarPathFinder.InsertRangeAttackPositions(reachableAttackPositions, grid, gridPos, 2, true);
            }
            else
            {
                AStarPathFinder.InsertMeleeAttackPositions(reachableAttackPositions, grid, gridPos);
            }

            foreach(var pos in reachableAttackPositions)
            {
                Tile3d tile = grid.GetTileAtGridPosition(pos);
                if (tile != null && tile.GetContainedEntity() != null && tile.GetContainedEntity().GetSide() != unit.GetSide())
                {
                    targetedEnemy = tile.GetContainedEntity();
                    return true;
                }
            }
            return false;
        }

        public void EnemyAttack()
        {
            Debug.Log("EL ENEMIGO VA A ATACAR");

        }
        public Status EndAttack()
        {
            Debug.Log("ENEMY HA ATACADO: SUCCESS");

            return Status.Success;
        }


        public bool IsEnemyInMovementRange()
        {
            allyUnit = turnSystem.GetAllyUnits();
            reachableMovePositions.Clear();

            targetedGridPosToMove = Vector3.zero;

            AStarPathFinder.InsertReachablePositionsAsyncBFS(
                reachableMovePositions,
                unit.Grid,
                unit.transform.position,
                unit.Stats.MovementStat
            );

            float bestDistance = float.MaxValue;
            Vector3 bestTile = default;

            foreach (var ally in allyUnit)
            {
                if (!unit.Grid.TryWorldToGridPosition(ally.transform.position, out Vector3 allyGridPos))
                    continue;

                Vector3[] dirs =
                {
                    new Vector3Int(1, 0, 0),
                    new Vector3Int(-1, 0, 0),
                    new Vector3Int(0, 0, 1),
                    new Vector3Int(0, 0, -1),
                };

                foreach (var dir in dirs)
                {
                    Vector3 adjacent = allyGridPos + dir;

                    if (!unit.Grid.IsGridPositionInBounds(adjacent))
                        continue;

                    if (!reachableMovePositions.Contains(adjacent))
                        continue;

                    unit.Grid.TryGridToWorldPosition(adjacent, out Vector3 worldPos);

                    float dist = Vector3.Distance(unit.transform.position, worldPos);

                    if (dist < bestDistance)
                    {
                        bestDistance = dist;
                        bestTile = adjacent;
                    }
                }
            }

            if (bestDistance == float.MaxValue)
                return false;

            targetedGridPosToMove = bestTile;

            unit.Grid.TryGridToWorldPosition(bestTile, out Vector3 targetWorld);
            targetedPosToMove = targetWorld.CenterOnTile();

            return true;
        }


        public void EnemyMove()
        {
            Debug.Log("EL ENEMIGO VA A MOVERSE A "+ targetedPosToMove + "DESDE "+ unit.transform.position);
            //unit.transform.position = targetedPosToMove; //test

            GetEntityComponent<MoveEntityComponent>().DoMove(targetedGridPosToMove, false);
        }

        public void SearchAndMoveToUnit()
        {
            Debug.Log("EL ENEMIGO VA A BUSCAR UNA UNIDAD Y MOVERSE HACIA ELLA");
        }

        public Status EndMove()
        {
            if (Vector3.Distance(unit.transform.position, targetedPosToMove) > 0.01f)
                return Status.Running;

            return Status.Success;
        }

        public void EndAITurn()
        {
            if (turnSystem != null)
            {
                startTurn = false;
                turnSystem.EndTurnForAIUnit(unit);
            }
        }
    }
}
