using System;

namespace CursedOnion.Game.Entity
{
    public class StatusHandler
    {
        SimpleEntity assignedEntity;
        ExtendedEntityStats stats;

        public SimpleEntity CounterAttackTarget;
        public int AdditionalHP;
        public bool IsConfused;
        public int ConfusedTurnsRemaining;
        public int AdditionalMovement;
        public float AttackMultiplier;

        public StatusHandler(SimpleEntity entityOwner, ExtendedEntityStats stats)
        {
            this.stats = stats;
            assignedEntity = entityOwner;
            
            AdditionalHP = 0;
            IsConfused = false;
            ConfusedTurnsRemaining = 0;
            AdditionalMovement = 0;
            AttackMultiplier = 1f;
        }
        
        #region Movement
        public void SetAdditionalMovement(int factor)
        {
            AdditionalMovement = factor;
        }
        private void ResetAdditionalMovement()
        {
            AdditionalMovement = 0;
        }
        #endregion

        #region AttackMultiplier
        public void SetAttackMultiplier(float multiplier)
        {
            AttackMultiplier = multiplier;
        }

        private void ResetAttackMultiplier()
        {
            AttackMultiplier = 1;
        }
        #endregion
        
        #region CounterAttack
        public bool HasCounterAttackTarget()
        {
            return CounterAttackTarget != null;
        }
        public bool HasCounterAttackTarget(out SimpleEntity counterAttackTarget)
        {
            counterAttackTarget = CounterAttackTarget;
            return CounterAttackTarget != null;
        }
        public void SetCounterAttackTarget(SimpleEntity target)
        {
            CounterAttackTarget = target;
        }
        public void ResetCounterAttack()
        {
            CounterAttackTarget = null;
        }

        #endregion
        
        #region Confusion
        public void ApplyConfusion(int turns)
        {
            IsConfused = true;
            ConfusedTurnsRemaining = turns;
            
            //assignedEntity.GetLayeredEntity().PlayAnimation("confusion");
        }
        private void DecreaseConfusion()
        {
            if (IsConfused)
            {
                ConfusedTurnsRemaining--;
                if (ConfusedTurnsRemaining <= 0)
                {
                    IsConfused = false;
                    //assignedEntity.GetLayeredEntity().PlayAnimation("idle");
                }
            }
        }
        #endregion
        
        #region AddtionalHP
        public void SetAdditionalHP(int factor)
        {
            AdditionalHP = stats.MaxHealthStat * factor / 100;
        }
        public int GetRemainingDamage(int damage)
        {
            int damageLeft = damage - AdditionalHP;
            
            AdditionalHP -= damage;
            if (AdditionalHP <= 0) AdditionalHP = 0;

            if (damageLeft <= 0) damageLeft = 0;
            return damageLeft;
        }
        private void ResetAdditionalHP()
        {
            AdditionalHP = 0;
        }
        #endregion
        public void UpdateStatusEffects()
        {
            ResetAttackMultiplier();
            ResetCounterAttack();
            DecreaseConfusion();
            ResetAdditionalHP();
            ResetAdditionalMovement();
        }
    }
}