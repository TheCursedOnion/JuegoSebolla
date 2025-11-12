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
    public class Unit : SimpleEntity
    {
        // Character UI
        [HorizontalLine(height: 2f, color: EColor.Violet)]
        
        [SerializeField] GameObject unitUI;
        public GameObject GetUI() => unitUI;

        [ReadOnly] public bool PlacedManually = false;

        public SpecialAbility SpecialAbility;

        [Inject] LevelManager levelManager;

        

        // Ability Status
        private float nextAttackMultiplier = 1;
        private int additionalHP = 0;
        private bool isConfused = false;
        private int confusedTurnsRemaining = 0;
        private int baseMovement;

        
        public void Start()
        {
            if (!PlacedManually)
            {
                SetLevelVariables();
                SetSide(EntitySide);
                AfterSpawn();
            }
        }
        
        public bool TrySpawningUnit(GameObject unitPrefab, Vector3 atPosition, BattleSide side)
        {
            SetLevelVariables();
            
            bool isPlaced = LevelManager.TryPlacingUnit(Data.GetPrice());
            if (isPlaced)
            {
                Spawn(unitPrefab, atPosition, side);
            }
            return isPlaced;
        }
        void Spawn(GameObject unitPrefab, Vector3 atPosition, BattleSide side)
        {
            Unit spawnedUnit = Instantiate(unitPrefab, atPosition, Quaternion.identity).GetComponent<Unit>();
            spawnedUnit.SetSide(side);
            spawnedUnit.PlacedManually = true;
            
            AfterSpawn();
        }
        void AfterSpawn()
        {
            Grid.GetTileAtWorldPosition(transform.position).SetContainedEntity(this);
            Stats.SetStats(Data);
            
            baseMovement = GetStats().MovementStat;

            var turnSystem = levelManager.GetTurnSystem();
            turnSystem.AddUnit(this);
            turnSystem.OnUnitTurnStart += HandleTurnStart;
            turnSystem.OnUnitTurnEnd += HandleTurnEnd;

            if (unitUI != null) unitUI.SetActive(false);
            
            InitializeAnimations();
            transform.localScale = new Vector3(0.75f, 0.75f, transform.localScale.z);
        }
        void SetSide(BattleSide side)
        {
            EntitySide = side;
            
            if(EntityController == null) Debug.LogWarning("EntityController es null, hay que cambiar esto por los controllers correspondientes");

            EntityController = Data.GetEntityController().Clone();
            EntityController ??= side switch
            {
                BattleSide.Enemy => EntityComponentController.Default,
                BattleSide.Ally => EntityComponentController.Default,
                _ => null
            };

            EntityController?.Initialize(this);
        }

        public bool TryErasingUnit(LevelManager manager)
        {
            bool canBeErased = PlacedManually && EntitySide == BattleSide.Ally;
            if (canBeErased)
            {
                manager.EraseUnit(Data.GetPrice());
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

        #region Special Ability

        
        #endregion

        #region Movement

        
        #endregion
    }
}
