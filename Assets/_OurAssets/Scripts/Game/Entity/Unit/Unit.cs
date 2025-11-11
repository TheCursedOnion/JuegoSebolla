using CursedOnion.Extensions;
using CursedOnion.Game.Entity.UI;
using CursedOnion.Game.Events;
using CursedOnion.Game.Modes.General.Animations;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.Game.Systems.Level;
using CursedOnion.Locators;
using CursedOnion.ScriptableObjects;
using NaughtyAttributes;
using Reflex.Attributes;
using Reflex.Core;
using Reflex.Extensions;
using Reflex.Injectors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
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

        private LayeredEntity layeredEntity;

        [Inject] LevelManager levelManager;

        public BattleSide Side;

        // Ability Status
        private float nextAttackMultiplier = 1;
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

            var camera = this.gameObject.scene.GetSceneContainer().Resolve<CameraLocator>().GlobalCamera.Camera;
            
            Grid.GetTileAtWorldPosition(transform.position).SetContainedEntity(this);

            //Debug.Log("El set de stats es temporal");
            Stats.SetStats(Data);
            baseMovement = GetStats().MovementStat;

            if (UnitController == null)
            {
                SetSide(Side);
            }

            var turnSystem = levelManager.GetTurnSystem();
            turnSystem.AddUnit(this);
            turnSystem.OnUnitTurnStart += HandleTurnStart;
            turnSystem.OnUnitTurnEnd += HandleTurnEnd;

            if (unitUI != null)
                unitUI.SetActive(false);
            InitializeAnimations();
            transform.localScale = new Vector3(0.75f, 0.75f, transform.localScale.z);
        }

        private void InitializeAnimations()
        {
            layeredEntity = GetComponent<LayeredEntity>();
            if (layeredEntity == null)
                layeredEntity = gameObject.AddComponent<LayeredEntity>();

            var animationGroups = GetStats().AnimationLayers;

            if (animationGroups == null || animationGroups.Count == 0)
            {
                Debug.LogWarning($"{name}: No se asignaron grupos de animación.");
                return;
            }

            var currentGroup = animationGroups[1]; // o el que quieras seleccionar

            if (currentGroup.layers == null || currentGroup.layers.Count == 0)
            {
                Debug.LogWarning($"{name}: El grupo '{currentGroup.groupName}' no tiene capas asignadas.");
                return;
            }

            layeredEntity.layers = currentGroup.layers;

            var initLayersMethod = typeof(LayeredEntity)
                .GetMethod("InitializeLayers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            initLayersMethod?.Invoke(layeredEntity, null);
        }
        private void HandleTurnStart(Unit unit)
        {
            if (unit == this)
            {
                //Debug.Log($"{name} puede actuar, mostrando su UI");
                if (unitUI != null)
                    unitUI.SetActive(true);
            }
        }

        private void HandleTurnEnd(Unit unit)
        {
            if (unit == this)
            {
                //Debug.Log($"{name} termina su turno, ocultando su UI");
                if (unitUI != null)
                    unitUI.SetActive(false);
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

        private void OnDestroy()
        {
            if (levelManager != null)
            {
                var turnSystem = levelManager.GetTurnSystem();
                turnSystem.OnUnitTurnStart -= HandleTurnStart;
                turnSystem.OnUnitTurnEnd -= HandleTurnEnd;
            }
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
            if (Stats.CurrentHealthStat <= 0)
            { 
                levelManager.GetTurnSystem().RemoveUnit(this);
                
                Die(); 
            }

        }

        public override void Heal(int healedHP)
        {
            Stats.CurrentHealthStat = Math.Min(Stats.CurrentHealthStat + healedHP, Stats.MaxHealthStat);
            Debug.Log($"{name} se cura {healedHP} de HP.");
        }
        #endregion

        #region Attack
        public void SetNextAttackMultiplier(float multiplier)
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

                int rawDamage = Mathf.CeilToInt(GetStats().AttackStat * nextAttackMultiplier);

                int targetDefense = targetedUnit.GetStats().DefenseStat;
                int finalDamage = Mathf.Max(1, rawDamage - targetDefense);

                Debug.Log($"{name} ataca a {targetedUnit.name} causando {finalDamage} de daño.");

                targetedUnit.Damage(finalDamage);

                nextAttackMultiplier = 1;


                if (targetedUnit.GetStats().CurrentHealthStat > 0 && GetStats().SpecialAbilityType is not ArcherAbility)
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

            if (target is Unit targetedUnit && (targetedUnit.Side == Side || targetedUnit.Side == BattleSide.Neutral ))
            {
                Grid.ResetPaint();
                Debug.LogWarning($"{name} no puede atacar a {target.name} porque son del mismo bando o es un elemento neutral.");
                return false;
            }

            if (!Grid.TryWorldToGridPosition(target.transform.position, out Vector3 targetGridPos))
                return false;

            var reachable = new List<Vector3>();

            if (GetStats().SpecialAbilityType is ArcherAbility)
            {
                reachable = Grid.GetReachablePositions(transform.position, 2, 2);
            }
            else
            {
                reachable = Grid.GetReachablePositions(transform.position, 1, 1);
            }
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

            var reachable = Grid.GetReachablePositions(transform.position, GetStats().SpecialAbilityType.AbilityMinRange, GetStats().SpecialAbilityType.AbilityMaxRange);

            var ability = GetStats().SpecialAbilityType;

            if (ability is ArcherAbility)
            {
                if (!Grid.TryWorldToGridPosition(transform.position, out Vector3 unitGridPos))
                    return false;

                Vector3 dir = targetGridPos - unitGridPos;
                dir.y = 0;

                if (Mathf.Abs(dir.x) > 0 && Mathf.Abs(dir.z) > 0)
                {
                    Grid.ResetPaint();
                    return false;
                }
            }

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
                layeredEntity.PlayAnimation("walk");
                StartCoroutine(MoveAlongPath(path));
            }
        }

        public override bool ValidateMove(Vector3 newPosition)
        {
            int moveRange = GetStats().MovementStat;

            var reachable = Grid.GetReachablePositionsMovement(transform.position, moveRange);

            Vector3Int target = newPosition.CastToVectorInt();
            Grid.ResetPaint();
            return reachable.Contains(target);

        }

        private IEnumerator MoveAlongPath(List<Vector3> path)
        {
            Grid.GetTileAtWorldPosition(transform.position).SetContainedEntity(null);

            float speed = 5f;
            Vector3 lastPosition = transform.position;

            foreach (var pos in path)
            {
                Vector3 direction = pos - lastPosition;

                if (direction.x > 0.01f)
                {
                    Vector3 scale = transform.localScale;
                    scale.x = Mathf.Abs(scale.x) * -1f;
                    transform.localScale = scale;
                }
                else if (direction.x < -0.01f)
                {
                    Vector3 scale = transform.localScale;
                    scale.x = Mathf.Abs(scale.x);
                    transform.localScale = scale;
                }

                while (Vector3.Distance(transform.position, pos) > 0.01f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, pos, speed * Time.deltaTime);
                    yield return null;
                }

                transform.position = pos;
                lastPosition = pos;
                yield return null;
            }
            layeredEntity.PlayAnimation("idle");
            Grid.GetTileAtWorldPosition(transform.position).SetContainedEntity(this);
        }
        #endregion
    }
}
