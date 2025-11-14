using CursedOnion.Game.Modes.General.Animations;
using CursedOnion.Game.Systems.Level;
using NaughtyAttributes;
using Reflex.Attributes;
using System;
using Reflex.Core;
using Reflex.Extensions;
using UnityEngine;

namespace CursedOnion.Game.Entity
{
    public enum BattleSide
    {
        Neutral,
        Ally,
        Enemy
    }
    public class Unit : SimpleEntity
    {
        // Character UI
        [HorizontalLine(height: 2f, color: EColor.Violet)]
        
        [SerializeField] GameObject unitUI;
        public GameObject GetUI() => unitUI;

        [ReadOnly] public bool PlacedManually = false;

        public SpecialAbility SpecialAbility;
        
        // Ability Status
        private int additionalHP = 0;
        private bool isConfused = false;
        private int confusedTurnsRemaining = 0;
        private int baseMovement;

        
        public void Start()
        {
            if (!PlacedManually)
            {
                SetLevelVariables(LevelManager);
                SetSide(EntitySide);
                SetComponents();
                AfterSpawn();
            }
        }
        
        public bool TrySpawningUnit(LevelManager levelManager, GameObject unitPrefab, Vector3 atPosition, BattleSide side)
        {
            bool isPlaced = levelManager.TryPlacingUnit(Data.GetPrice());
            if (isPlaced)
            {
                Spawn(levelManager, unitPrefab, atPosition, side);
            }
            return isPlaced;
        }
        void Spawn(LevelManager levelManager, GameObject unitPrefab, Vector3 atPosition, BattleSide side)
        {
            Unit spawnedUnit = Instantiate(unitPrefab, atPosition, Quaternion.identity).GetComponent<Unit>();
            spawnedUnit.SetLevelVariables(levelManager);
            spawnedUnit.PlacedManually = true;
            spawnedUnit.SetSide(side);
            spawnedUnit.SetComponents();
            spawnedUnit.AfterSpawn();
        }
        void SetSide(BattleSide side)
        {
            EntitySide = side;
        }
        void SetComponents()
        {
            switch(GetSide())
            {
                case BattleSide.Ally:
                    EntityController ??= gameObject.AddComponent<PlayerUnitController>();
                    break;
                case BattleSide.Enemy:
                    EntityController ??= gameObject.AddComponent<AIUnitController>();
                    break;
                default:
                    //EntityController ??= gameObject.AddComponent<EntityComponentController>();
                    break;
            }
            EntityController.Initialize(this, Data.GetEntityComponents());
        }
        void AfterSpawn()
        {
            Stats.SetStats(Data);
            
            baseMovement = GetStats().MovementStat;

            if (unitUI != null) unitUI.SetActive(false);
            
            InitializeAnimations();
            transform.localScale = new Vector3(0.75f, 0.75f, transform.localScale.z);
        }

        public bool TryErasingUnit()
        {
            bool canBeErased = PlacedManually && EntitySide == BattleSide.Ally && LevelManager != null;
            if (canBeErased)
            {
                Debug.Log($"{name} se ha eliminado.");
                LevelManager.EraseUnit(Data.GetPrice());
                Dispose();
            }
            return canBeErased;
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
            
            layeredEntity.InitializeLayers(currentGroup);
        }
        /*private void HandleTurnStart(Unit unit)
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
        }*/

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
            if (Stats.CurrentHealthStat <= 0)
            { 
                LevelManager.GetTurnSystem().RemoveUnit(this);
                
                Die(); 
            }

        }

        public override void Heal(int healedHP)
        {
            Stats.CurrentHealthStat = Math.Min(Stats.CurrentHealthStat + healedHP, Stats.MaxHealthStat);
            Debug.Log($"{name} se cura {healedHP} de HP.");
        }
        #endregion
    }
}
