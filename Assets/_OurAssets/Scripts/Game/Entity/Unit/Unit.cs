using CursedOnion.Extensions;
using CursedOnion.Game.Entity.UI;
using CursedOnion.Game.Events;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.Game.Systems.Level;
using CursedOnion.ScriptableObjects;
using NaughtyAttributes;
using Reflex.Core;
using Reflex.Extensions;
using Reflex.Injectors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.TestTools;

namespace CursedOnion.Game.Entity
{
    public enum BattleSide
    {
        Neutral,
        Ally,
        Enemy
    }
    public class Unit : CommandableEntity
    {
        // Character UI
        [SerializeField] GameObject unitUI;
        public GameObject GetUI() => unitUI;

        [ReadOnly] public bool PlacedManually = false;

        public UnitController UnitController;

        public SpecialAbility SpecialAbility;

        public BattleSide Side;

        // Ability Status
        private int nextAttackMultiplier = 1;
        private int additionalHP = 0;
        private bool isConfused = false;
        private int confusedTurnsRemaining = 0;
        private int baseMovement;

        public bool TrySpawningUnit(LevelManager manager, GameObject unitPrefab, Vector3 atPosition, BattleSide side)
        {
            SetLevelVariables(manager);

            bool isPlaced = LevelManager.TryPlacingUnit(Data.GetPrice());
            if (isPlaced)
            {
                Unit spawnedUnit = Instantiate(unitPrefab, atPosition, Quaternion.identity).GetComponent<Unit>();
                spawnedUnit.SetSide(side);
                spawnedUnit.PlacedManually = true;
            }
            return isPlaced;
        }
        void SetSide(BattleSide side)
        {
            Side = side;

            if (UnitController != null) Destroy(UnitController);

            UnitController = Side switch
            {
                BattleSide.Enemy => gameObject.AddComponent<AIUnitController>(),
                BattleSide.Ally => gameObject.AddComponent<PlayerUnitController>(),
                _ => null
            };
        }

        public bool TryErasingUnit(LevelManager manager)
        {
            bool canBeErased = PlacedManually && Side == BattleSide.Ally;
            if (canBeErased)
            {
                manager.EraseUnit(Data.GetPrice());
                Dispose();
            }
            return canBeErased;
        }

        public void Start()
        {
            var container = this.gameObject.scene.GetSceneContainer();
            SetLevelVariables(container.Resolve<LevelManager>());

            Grid.GetTileAtWorldPosition(transform.position).SetContainedEntity(this);

            //Debug.Log("El set de stats es temporal");
            Stats.SetStats(Data);
            baseMovement = GetStats().MovementStat;

            if (UnitController == null)
            {
                SetSide(Side);
            }
        }

        public Grid3d GetGrid()
        {
            return Grid;
        }

        public void ApplyConfusion(int turns)
        {
            isConfused = true;
            confusedTurnsRemaining = turns;
        }

        public void UpdateStatusEffects()
        {
            if (isConfused)
            {
                confusedTurnsRemaining--;
                if (confusedTurnsRemaining <= 0)
                {
                    isConfused = false;
                }
            }
            this.GetStats().MovementStat = baseMovement;
        }
        #region Health
        public void SetAdditionalHP(int factor)
        {
            additionalHP = GetStats().MaxHealthStat * factor / 100;
            Debug.Log($"{name} recibe {additionalHP} de HP adicional.");
        }

        public override void Damage(int damage)
        {
            Debug.Log($"{name} recibe {damage} de daño.");

            if (additionalHP > 0)
            {
                if (damage <= additionalHP)
                {
                    additionalHP -= damage;
                    return;
                }
                else
                {
                    damage -= Mathf.FloorToInt(additionalHP);
                    additionalHP = 0;
                }
            }

            Stats.CurrentHealthStat -= damage;
            if (Stats.CurrentHealthStat <= 0) Die();
        }

        public override void Heal(int healedHP)
        {
            Stats.CurrentHealthStat = Math.Min(Stats.CurrentHealthStat + healedHP, Stats.MaxHealthStat);
            Debug.Log($"{name} se cura {healedHP} de HP.");
        }
        #endregion

        #region Attack
        public void SetNextAttackMultiplier(int multiplier)
        {
            nextAttackMultiplier = multiplier;
        }

        protected override void DoAttack(SimpleEntity target, bool undo)
        {
            if (target is Unit targetedUnit)
            {
                if (targetedUnit.Side == Side)
                {
                    Debug.LogWarning($"{name} no puede atacar a {target.name} porque son del mismo bando.");
                    return;
                }

                Grid.ResetPaint();

                int rawDamage = GetStats().AttackStat * nextAttackMultiplier;
                int targetDefense = targetedUnit.GetStats().DefenseStat;
                int finalDamage = Mathf.Max(1, rawDamage - targetDefense);

                Debug.Log($"{name} ataca a {targetedUnit.name} causando {finalDamage} de daño.");

                targetedUnit.Damage(finalDamage);

                nextAttackMultiplier = 1;


                if (targetedUnit.GetStats().CurrentHealthStat > 0)
                {
                    int counterDamage = targetedUnit.GetStats().AttackStat;

                    Debug.Log($"{targetedUnit.name} contraataca a {name} causando {counterDamage} de daño.");

                    Damage(counterDamage);
                }
            }
        }

        public override bool ValidateAttack(SimpleEntity target)
        {
            if (target == null)
            {
                Grid.ResetPaint();
                Debug.LogWarning("ValidateAttack falló: target es null");
                return false;
            }

            if (!Grid.TryWorldToGridPosition(target.transform.position, out Vector3 targetGridPos))
                return false;

            var reachable = Grid.GetReachablePositions(transform.position, 1);
            Grid.ResetPaint();
            return reachable.Contains(targetGridPos);
        }
        #endregion

        #region Special Ability
       
        protected override void DoAbility(SimpleEntity target, bool undo)
        {
            Grid.ResetPaint();
            GetStats().SpecialAbilityType.ActivateAbility(this, target);
            Debug.Log($"{gameObject.name} usa su habilidad en {target.gameObject.name}");
        }

        public override bool ValidateAbility(SimpleEntity target)
        {
            if (GetStats().SpecialAbilityType.SelfTargetOnly == true)
            {
                if (target == (SimpleEntity)this)
                    return true;

                Debug.LogWarning($"[ValidateAbility] {name} tiene SelfTargetOnly pero el target no es el mismo objeto.");
                return false;
            }

            if (target == null)
            {
                Grid.ResetPaint();
                Debug.LogWarning("ValidateAbility falló: target es null");
                return false;
            }

            if (!Grid.TryWorldToGridPosition(target.transform.position, out Vector3 targetGridPos))
                return false;

            var reachable = Grid.GetReachablePositions(transform.position, SpecialAbility.AbilityRange);
            Grid.ResetPaint();
            return reachable.Contains(targetGridPos);
        }
        #endregion

        #region Movement
        protected override void DoMove(Vector3 newPosition, bool undo)
        {

            if (undo)
            {
                transform.position = newPosition;
            }
            else
            {
                Debug.Log($"{gameObject.name}: Me muevo a {newPosition}");

                if (!Grid.TryWorldToGridPosition(transform.position, out Vector3 startGrid))
                {
                    Debug.LogError($"TryWorldToGridPosition falló para start world position: {transform.position}");
                    return;
                }

                var path = UnitController.GetPathFinder().FindPath(startGrid, newPosition, Grid);

                if (path == null || path.Count == 0)
                {
                    Grid.ResetPaint();
                    Debug.LogWarning("No se encontró camino (FindPath devolvió null/empty).");
                    return;
                }
                Grid.ResetPaint();
                GetStats().MovementStat = baseMovement;
                StartCoroutine(MoveAlongPath(path));
            }
        }

        public override bool ValidateMove(Vector3 newPosition)
        {
            int moveRange = GetStats().MovementStat;

            var reachable = Grid.GetReachablePositions(transform.position, moveRange);

            Vector3Int target = newPosition.CastToVectorInt();
            Grid.ResetPaint();
            return reachable.Contains(target);

        }

        private IEnumerator MoveAlongPath(List<Vector3> path)
        {
            Grid.GetTileAtWorldPosition(transform.position).SetContainedEntity(null);

            float speed = 5f;

            foreach (var pos in path)
            {
                while (Vector3.Distance(transform.position, pos) > 0.01f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, pos, speed * Time.deltaTime);
                    yield return null;
                }

                transform.position = pos;
                yield return new WaitForSeconds(0.05f);
            }
            Grid.GetTileAtWorldPosition(transform.position).SetContainedEntity(this);
        }
        #endregion
    }
}
