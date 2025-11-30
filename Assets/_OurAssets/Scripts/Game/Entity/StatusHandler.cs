using System;
using System.Collections.Generic;
using CursedOnion.Game.Entity.Effects;
using UnityEngine;

namespace CursedOnion.Game.Entity
{
    public class StatusHandler
    {
        SimpleEntity assignedEntity;
        ExtendedEntityStats stats;

        public SimpleEntity CounterAttackTarget;
        
        public int AdditionalHP;
        public int MaxAdditionalHP;
        
        public bool IsConfused;
        public int AdditionalMovement;
        public float AttackMultiplier;

        private readonly List<StatusEffect> effects = new();
        public StatusHandler(SimpleEntity entityOwner, ExtendedEntityStats stats)
        {
            this.stats = stats;
            assignedEntity = entityOwner;
            
            AdditionalHP = 0;
            MaxAdditionalHP = 1;
            
            IsConfused = false;
            AdditionalMovement = 0;
            AttackMultiplier = 1f;
        }
        public List<StatusEffect> GetActiveEffects() => effects;
        public void AddEffect(StatusEffect newEffect)
        {
            var existing = effects.Find(e => e.GetType() == newEffect.GetType());
            
            if (existing == null)
            {
                //newEffect.OnStart += stats.OnEffectStart;
                //newEffect.OnEnd += stats.OnEffectEnd;
                effects.Add(newEffect);
                newEffect.ApplyOn(this);
            }
            else
            {
                existing.ResetDuration(newEffect.GetDuration());
            }
        }
        
        #region Movement
        public bool HasAdditionalMovement()
        {
            return AdditionalMovement > 0;
        }
        #endregion

        #region AttackMultiplier
        public bool HasAttackMultiplier()
        {
            return AttackMultiplier > 1;
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
        public bool HasConfusionEffect()
        {
            return IsConfused;
        }
        #endregion
        
        #region AddtionalHP
        public void SetAdditionalHP(float factor)
        {
            MaxAdditionalHP = Mathf.RoundToInt(stats.MaxHealthStat * factor);
            AdditionalHP = MaxAdditionalHP;
        }
        public bool HasAdditionalHP()
        {
            return AdditionalHP > 0;
        }
        public int GetRemainingDamage(int damage)
        {
            int damageLeft = damage - AdditionalHP;
            
            AdditionalHP -= damage;
            if (AdditionalHP <= 0) AdditionalHP = 0;

            if (damageLeft <= 0) damageLeft = 0;
            return damageLeft;
        }
        #endregion
        public void UpdateStatusEffects()
        {
            ResetCounterAttack();

            foreach (var effect in effects.ToArray())
            {
                bool ended = effect.UpdateEffect(this);
                if (ended) effects.Remove(effect);
            }
        }
    }
}