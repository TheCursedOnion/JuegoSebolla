using CursedOnion.Extensions;
using CursedOnion.Game.Cameras;
using CursedOnion.Game.Entity.Components;
using CursedOnion.Game.Modes.General.Animations;
using CursedOnion.Game.Systems.Level;
using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;

namespace CursedOnion.Game.Entity
{
    public enum BattleSide
    {
        Neutral = -1,
        Ally = 0,
        Enemy = 1,
    }
    public class Unit : SimpleEntity
    {
        // Character UI
        [HorizontalLine(height: 2f, color: EColor.Violet)] 
        [SerializeField] GameObject unitUI;
        public GameObject GetUI() => unitUI;

        [ReadOnly] public bool PlacedManually = false;
        [SerializeField, ReadOnly] CameraFocus cameraFocus;
        public bool IsBoss;
        
        protected override void Awake()
        {
            Stats = new ExtendedEntityStats();
        }

        protected override void Start()
        {
            if (!PlacedManually)
            {
                DefineEntityStats(StatData);
                SetLevelVariables(LevelManager);
                SetSide(EntitySide);
            }
            
            SetComponents();
            AfterSpawn();
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
            DefineEntityStats(data);
            SetLevelVariables(levelManager);
            SetSide(side);
        }

        void SetSide(BattleSide side)
        {
            EntitySide = side;
        }
        
        void AfterSpawn()
        {
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
            LayeredEntity = GetComponent<LayeredEntity>();
            if (LayeredEntity == null) LayeredEntity = gameObject.AddComponent<LayeredEntity>();
            
            int periodId = (int)LevelManager.LevelAsset.LevelData.TimePeriod * 2;
            int indexOffset = (int)EntitySide;
            
            Debug.Log($"Period: {periodId}, Index: {indexOffset} y {EntitySide}");
            
            LayeredEntity.Initialize(Stats.AnimationLayers, periodId + indexOffset);
        }

        #region Damage
        public override void DamageFrom(int damage, SimpleEntity attacker)
        {
            LayeredEntity?.PlayAnimation("hurt");
            base.DamageFrom(damage, attacker);

            if (Stats.CurrentHealthStat > 0)
            {
                if (this.ActionHandler.CanCounter()
                    && this.Stats.SpecialAbilityType is not ArcherAbility
                    && attacker is Unit attackerUnit
                    && attackerUnit.Stats.SpecialAbilityType is not ArcherAbility
                    )
                {
                    StatusHandler.SetCounterAttackTarget(attacker);
                }
            }
        }
        #endregion

        public void FocusOnUnit()
        {
            switch (EntitySide)
            {
                case BattleSide.Enemy:
                    cameraFocus.RequestFocus();
                    break;
                
                case BattleSide.Ally:
                default: 
                    CheckUnit();
                    break;
            }
        }

        public void CheckUnit()
        {
            LevelEvents.InvokeTurnFocus(this);
        }
    }
}
