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
            LayeredEntity = GetComponent<LayeredEntity>();
            if (LayeredEntity == null) LayeredEntity = gameObject.AddComponent<LayeredEntity>();
            
            int periodId = (int)LevelManager.LevelAsset.LevelData.TimePeriod * 2;
            int indexOffset = (int)EntitySide;
            
            Debug.Log($"Period: {periodId}, Index: {indexOffset} y {EntitySide}");
            LayeredEntity.Initialize(Stats.AnimationLayers, periodId + indexOffset);
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

        public override void Damage(int damage, Action onDamageAnimationFinished = null)
        {
            Debug.Log($"{name} recibe {damage} de daño.");

            if (additionalHP > 0)
            {
                if (damage <= additionalHP)
                {
                    additionalHP -= damage;
                    onDamageAnimationFinished?.Invoke();
                    return;
                }
                else
                {
                    damage -= Mathf.FloorToInt(additionalHP);
                    additionalHP = 0;
                }
            }

            Stats.CurrentHealthStat -= damage;

            if (TryGetLayeredEntity(out var layeredEntity))
            {
                layeredEntity.PlayAnimation("hurt");
            }

            if (Stats.CurrentHealthStat <= 0)
            {
                LevelManager.GetTurnSystem().RemoveUnit(this);
                if(EntityController is not PlayerUnitController)
                {
                    //EntityController.EndAITurn();
                }
                Die();
            }

            StartCoroutine(InvokeAfterDelay(0.5f, onDamageAnimationFinished));

        }

        private IEnumerator InvokeAfterDelay(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback?.Invoke();
        }

        public override void Heal(int healedHP)
        {
            Stats.CurrentHealthStat = Math.Min(Stats.CurrentHealthStat + healedHP, Stats.MaxHealthStat);
            Debug.Log($"{name} se cura {healedHP} de HP.");
        }
        #endregion
    }
}
