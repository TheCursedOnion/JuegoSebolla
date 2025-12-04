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
    public class AIJeanneBossController : MonoBehaviour
    {
        List<Vector3> jeanneReachableTiles = new();

        bool hasTeleportedFirst = false;
        bool hasTeleportedSecond = false;

        AIUnitController baseAI;

        SimpleEntity enemyTarget;

        //garantiza que el baseAI esté listo
        private void LazyInit()
        {
            if (baseAI != null) return;

            baseAI = GetComponent<AIUnitController>();
            if (baseAI == null)
                Debug.LogError("AIJeanneBossController NO encontró AIUnitController en el mismo GameObject.");
        }


        #region Perceptions

       

        public bool FirstTPCondition()
        {
            LazyInit();
            if (hasTeleportedFirst) return false;

            var unit = baseAI.GetUnit();
            float hpPercent = unit.Stats.CurrentHealthStat / (float)unit.Stats.MaxHealthStat;
            Debug.Log($"BossJeanne HP%: {hpPercent * 100}%");
            return hpPercent <= 0.75f;
        }

        public bool SecondTPCondition()
        {
            LazyInit();
            if (hasTeleportedSecond) return false;

            var unit = baseAI.GetUnit();
            float hpPercent = unit.Stats.CurrentHealthStat / (float)unit.Stats.MaxHealthStat;
            Debug.Log($"BossJeanne HP%: {hpPercent * 100}%");
            return hpPercent <= 0.40f;
        }
        #endregion

        #region ActionLogic

        public void StartTPAnimation()
        {
            var entity = baseAI.GetUnit() as SimpleEntity;
            baseAI.GetEntityComponent<SpecialAbilityComponent>().DoAbility(entity, false);
        }

        public void TeleportToFirstPosition()
        {
            hasTeleportedFirst = true;
            Debug.Log("BossJeanne teleported to first position");
            Vector3 firstPos = new Vector3(0.5f, 2, 1.5f); 
            TeleportInstant(firstPos);
        }

        public void TeleportToSecondPosition()
        {
            hasTeleportedSecond = true;
            Debug.Log("BossJeanne teleported to second position");
            Vector3 secondPos = new Vector3(0.5f, 1, -1.5f);
            TeleportInstant(secondPos);
        }

        private void TeleportInstant(Vector3 targetWorldPos)
        {
            var unit = baseAI.GetUnit();
            var transform = unit.transform;

            var placeComp = baseAI.GetEntityComponent<PlaceEntityComponent>();
            placeComp.RemoveEntity();

            transform.position = targetWorldPos.CenterOnTile();

            placeComp.PlaceEntity();

            Debug.Log($"[Teleport] {unit.name} ⇒ {targetWorldPos}");

            if (unit.TryGetLayeredEntity(out var layered))
                layered.PlayAnimation("idle");
        }

        #endregion

        #region UtilitySystems

        public void SelectBestEnemyToAttack()
        {
            LazyInit();

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
            Debug.Log("Soldier selected best enemy to attack: " + enemyTarget?.name);
        }

        public void SelectBestTileNearEnemy()
        {
            LazyInit();

            if (enemyTarget == null) return;

            var unit = baseAI.GetUnit();
            jeanneReachableTiles.Clear();

            _ = AStarPathFinder.InsertReachableGridPositionsAsyncBFS(
                jeanneReachableTiles,
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
                if (!jeanneReachableTiles.Contains(t)) continue;

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
