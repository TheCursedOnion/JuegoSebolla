using System;
using UnityEngine;

namespace CursedOnion.Game.Entity
{
    public class SimpleEntity : MonoBehaviour
    {
        public Action OnEntityUpdate;

        public EntityData Data;
        protected virtual EntityStats Stats { get; } = new EntityStats();
        public EntityStats GetStats() => Stats;
        
        protected bool IsDead = false;
        public bool HasDied() => IsDead;

        public virtual void Damage(int damage)
        {
            Stats.CurrentHealthStat -= damage;
            if (Stats.CurrentHealthStat <= 0) Die();
        }
        public virtual void Die()
        {
            IsDead = true;
        }

        public virtual void Revive(int newHealth)
        {
            Stats.CurrentHealthStat = newHealth;
            IsDead = false;
        }

        public virtual void DebugEntity(){}
    }
    
    public abstract class CommandableEntity : SimpleEntity
    {
        [SerializeField] 
        protected override EntityStats Stats { get; } = new ExtendedEntityStats();
        public new ExtendedEntityStats GetStats() => Stats as ExtendedEntityStats;

        #region Basic Commands
        public void Attack(SimpleEntity target)
        {
            DoAttack(target, undo: false);
        }

        public void UndoAttack(SimpleEntity target)
        {
            DoAttack(target, undo: true);
        }
        
        protected abstract void DoAttack(SimpleEntity target, bool undo);
        public abstract bool ValidateAttack(SimpleEntity target);
        
        public void Move(Vector3 newPosition)
        {
            DoMove(newPosition, undo: false);
        }

        public void UndoMove(Vector3 previousPosition)
        {
            DoMove(previousPosition, undo: true);
        }
        protected abstract void DoMove(Vector3 newPosition, bool undo);
        public abstract bool ValidateMove(Vector3 newPosition);
        
        #endregion
    }
}
