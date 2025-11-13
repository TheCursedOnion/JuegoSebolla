using System;

namespace CursedOnion.Game.Entity
{
    [Flags]
    public enum EntityFlag
    {
        None        = 0,
        
        HasDied     = 1 << 0,
        HasMoved    = 1 << 1,
        HasAttacked = 1 << 2,
        HasUsedAbility = 1 << 3,
    }
    public class EntityFlags
    {
        private EntityFlag currentFlags;
        private readonly SimpleEntity entityOwner;
        public EntityFlags(SimpleEntity entityOwner)
        {
            currentFlags = EntityFlag.None;
            this.entityOwner = entityOwner;
        }

        public bool HasDied() => HasFlagRaised(EntityFlag.HasDied);
        public bool HasMoved() => HasFlagRaised(EntityFlag.HasMoved);
        public bool HasAttacked() => HasFlagRaised(EntityFlag.HasAttacked);
        public bool HasUsedAbility() => HasFlagRaised(EntityFlag.HasUsedAbility);
        public bool HasSpentAllActions() => HasAttacked() && HasUsedAbility() && HasMoved();
        bool HasFlagRaised(EntityFlag flag) => (currentFlags & flag) != 0;
        
        public void RaiseFlag(EntityFlag flag)
        {
            currentFlags |= flag;
            entityOwner.NotifyUpdate();
        }
        public void ResetFlag(EntityFlag flag)
        {
            currentFlags &= ~flag;
            entityOwner.NotifyUpdate();
        }
    }
}