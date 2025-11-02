using System;
using UnityEngine;

namespace CursedOnion.Game.Entity
{
    public class SimpleEntity : MonoBehaviour
    {
        public Action OnEntityUpdate;

        public EntityData Data;
        
        //Stats (They Get Defined)
        protected virtual EntityStats Stats { get; } = new EntityStats();
        public EntityStats GetStats() => Stats;
        
        //Flags
        protected virtual EntityFlags Flags { get; } = new EntityFlags();
        public EntityFlags GetFlags() => Flags;

        public virtual void Damage(int damage)
        {
            Stats.CurrentHealthStat -= damage;
            if (Stats.CurrentHealthStat <= 0) Die();
        }
        public virtual void Die()
        {
            GetFlags().HasDied = true;
            OnEntityUpdate?.Invoke();
            Dispose();
        }

        public virtual void Revive(int newHealth)
        {
            Stats.CurrentHealthStat = newHealth;
            GetFlags().HasDied = false;
            
            OnEntityUpdate?.Invoke();
        }
        
        public void Dispose()
        {
            Destroy(gameObject);
        }
    }
    
    
    public abstract class CommandableEntity : SimpleEntity
    {
        //Stats (They Get Defined)
        protected override EntityStats Stats { get; } = new ExtendedEntityStats();
        public new ExtendedEntityStats GetStats() => Stats as ExtendedEntityStats;
        
        //Flags
        protected override EntityFlags Flags { get; } = new ExtendedEntityFlags();
        public new ExtendedEntityFlags GetFlags() => Flags as ExtendedEntityFlags;
        
        
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
