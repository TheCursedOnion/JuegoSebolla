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
        public Vector3 TargetedGridPosToMove;
        public Vector3 TargetedPosToMove;
        public SimpleEntity TargetedEnemy;

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
        
        #region AttackLogic
        public bool IsEnemyInAttackRange()
        {
            var grid = unit.Grid;
            var position = unit.transform.position;

            reachableAttackPositions.Clear();
            bool isMeleeUnit = unit.Stats.SpecialAbilityType is not ArcherAbility;

            grid.TryWorldToGridPosition(position, out Vector3 gridPos);
            if (!isMeleeUnit)
            {
                AStarPathFinder.InsertManhattanAttackGridPositions(reachableAttackPositions, grid, gridPos, 2, true);
            }
            else
            {
                AStarPathFinder.InsertMeleeAttackGridPositions(reachableAttackPositions, grid, gridPos);
            }

            foreach(var pos in reachableAttackPositions)
            {
                Tile3d tile = grid.GetTileAtGridPosition(pos);
                if (tile != null && tile.GetContainedEntity() != null && tile.GetContainedEntity().GetSide() != unit.GetSide())
                {
                    TargetedEnemy = tile.GetContainedEntity();
                    return true;
                }
            }
            return false;
        }

        public void EnemyAttack()
        {
            Debug.Log("EL ENEMIGO VA A ATACAR A" + TargetedEnemy);
            GetEntityComponent<AttackEntityComponent>().DoAttack(TargetedEnemy, false);


        }
        public Status EndAttack()
        {
            Debug.Log("ENEMY HA ATACADO: SUCCESS");
            TargetedEnemy = null;
            return Status.Success;
        }
        #endregion

        #region MovementLogic
        public bool IsEnemyInMovementRange()
        {
            allyUnit = turnSystem.GetAllyUnits();
            reachableMovePositions.Clear();

            TargetedGridPosToMove = Vector3.zero;
            TargetedEnemy = null;

            _ = AStarPathFinder.InsertReachableGridPositionsAsyncBFS(
                reachableMovePositions,
                unit.Grid,
                unit.transform.position,
                unit.Stats.MovementStat
            );

            float bestDistance = float.MaxValue;
            Vector3 bestTile = default;
            SimpleEntity bestTargetedEnemy = null;

            bool isMeleeUnit = unit.Stats.SpecialAbilityType is not ArcherAbility;

            foreach (var ally in allyUnit)
            {
                if (!unit.Grid.TryWorldToGridPosition(ally.transform.position, out Vector3 allyGridPos))
                    continue;

                List<Vector3> candidatePositions = new List<Vector3>();

                if (isMeleeUnit)
                {
                    // Posiciones adyacentes al aliado
                    Vector3[] dirs =
                    {
                        new Vector3(1, 0, 0),
                        new Vector3(-1, 0, 0),
                        new Vector3(0, 0, 1),
                        new Vector3(0, 0, -1),
                    };

                    foreach (var dir in dirs)
                    {
                        Vector3 adjacent = allyGridPos + dir;
                        if (unit.Grid.IsGridPositionInBounds(adjacent))
                            candidatePositions.Add(adjacent);
                    }
                }
                else
                {
                    // Para arqueros: posiciones desde las que puede atacar al aliado
                    AStarPathFinder.InsertManhattanAttackGridPositions(candidatePositions, unit.Grid, allyGridPos, 2, true);
                }

                foreach (var pos in candidatePositions)
                {
                    if (!reachableMovePositions.Contains(pos))
                        continue;

                    unit.Grid.TryGridToWorldPosition(pos, out Vector3 worldPos);

                    float dist = Vector3.Distance(unit.transform.position, worldPos);

                    if (dist < bestDistance)
                    {
                        bestDistance = dist;
                        bestTile = pos;
                        bestTargetedEnemy = ally;
                    }
                }
            }

            if (bestDistance == float.MaxValue)
                return false;

            TargetedGridPosToMove = bestTile;
            TargetedEnemy = bestTargetedEnemy;

            unit.Grid.TryGridToWorldPosition(bestTile, out Vector3 targetWorld);
            TargetedPosToMove = targetWorld.CenterOnTile();

            return true;
        }


        public void EnemyMove()
        {
            GetEntityComponent<MoveEntityComponent>().DoMove(TargetedGridPosToMove, false);
        }

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

            if (!unit.Grid.TryWorldToGridPosition(closestAlly.transform.position, out Vector3 allyGridPos))
                return false;

            reachableMovePositions.Clear();
            _ = AStarPathFinder.InsertReachableGridPositionsAsyncBFS(
                reachableMovePositions,
                unit.Grid,
                unit.transform.position,
                unit.Stats.MovementStat
            );

            if (reachableMovePositions.Count == 0)
                return false; 

            float bestTileDist = float.MaxValue;
            Vector3 bestTile = default;

            foreach (var reachable in reachableMovePositions)
            {
                float dist = Vector3.Distance(reachable, allyGridPos);
                if (dist < bestTileDist)
                {
                    bestTileDist = dist;
                    bestTile = reachable;
                }
            }

            TargetedGridPosToMove = bestTile;

            if (!unit.Grid.TryGridToWorldPosition(bestTile, out Vector3 targetWorld))
                return false;

            TargetedPosToMove = targetWorld.CenterOnTile();

            return true;
        }

        public Status EndMove()
        {
            Vector3 pos = unit.transform.position;
            Vector3 target = TargetedPosToMove;

            bool xzAligned = Mathf.Abs(pos.x - target.x) < 0.05f && Mathf.Abs(pos.z - target.z) < 0.05f;
            bool yCloseEnough = Mathf.Abs(pos.y - target.y) < 0.6f;

            if (!xzAligned || !yCloseEnough)
                return Status.Running;

            TargetedGridPosToMove = Vector3.zero;
            TargetedPosToMove = Vector3.zero;
            return Status.Success;
        }
        #endregion

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
