using CursedOnion.Game.Entity.Components;
using CursedOnion.Game.Modes.General.Animations;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.Game.Systems.Level;
using NaughtyAttributes;
using Reflex.Attributes;
using Reflex.Core;
using Reflex.Extensions;
using System;
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
        public int baseMovement;
        public float AttackMultiplier = 1f;
        
        public void Start()
        {
            if (!PlacedManually)
            {
                DefineStats(StatData);
                SetLevelVariables(LevelManager);
                SetSide(EntitySide);
                SetComponents();
                AfterSpawn();
            }
        }
        
        public bool TrySpawningUnit(LevelManager levelManager, GameObject unitPrefab, StatData data, Vector3 atPosition, BattleSide side)
        {
            bool isPlaced = levelManager.TryPlacingUnit(data.GetPrice());
            if (isPlaced)
            {
                Unit spawnedUnit = Instantiate(unitPrefab, atPosition, Quaternion.identity).GetComponent<Unit>();
                spawnedUnit.ManualInitialization(levelManager, data, side);
            }
            return isPlaced;
        }
        void ManualInitialization(LevelManager levelManager, StatData data, BattleSide side)
        {
            PlacedManually = true;
            DefineStats(data);
            SetLevelVariables(levelManager);
            SetSide(side);
            SetComponents();
            AfterSpawn();
        }

        protected override void DefineStats(StatData data)
        {
            base.DefineStats(data);
            AttackMultiplier = 1f;
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
            EntityController.Initialize(this, StatData.EntityComponents);
        }
        void AfterSpawn()
        {
            baseMovement = Stats.MovementStat;

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
                LevelManager.EraseUnit(StatData.GetPrice());
                Dispose();
            }
            return canBeErased;
        }
        private void InitializeAnimations()
        {
            layeredEntity = GetComponent<LayeredEntity>();
            if (layeredEntity == null)
                layeredEntity = gameObject.AddComponent<LayeredEntity>();

            var animationGroups = Stats.AnimationLayers;

            if (animationGroups == null || animationGroups.Count == 0)
            {
                Debug.LogWarning($"{name}: No se asignaron grupos de animación.");
                return;
            }

            var currentGroup = animationGroups[0]; // default

            switch (GetSide())// o el que quieras seleccionar
            {
                case BattleSide.Ally:
                    currentGroup = animationGroups[0];
                    break;
                case BattleSide.Enemy:
                    currentGroup = animationGroups[1];
                    break;
            }
                    

            if (currentGroup.layers == null || currentGroup.layers.Count == 0)
            {
                Debug.LogWarning($"{name}: El grupo '{currentGroup.groupName}' no tiene capas asignadas.");
                return;
            }
            
            layeredEntity.InitializeLayers(currentGroup);
        }

        public void ApplyConfusion(int turns)
        {
            isConfused = true;
            confusedTurnsRemaining = turns;
        }

        public void UpdateStatusEffects()
        {
            this.Stats.MovementStat = baseMovement;
        }

        #region Health
        public void SetAdditionalHP(int factor)
        {
            additionalHP = Stats.MaxHealthStat * factor / 100;
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

            if (TryGetLayeredEntity(out var layeredEntity)) layeredEntity.PlayAnimation("hurt");

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
