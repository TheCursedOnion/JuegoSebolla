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
        protected EntityFlag CurrentFlags;
        protected SimpleEntity Entity;
        public EntityFlags(SimpleEntity entity)
        {
            CurrentFlags = EntityFlag.None;
            Entity = entity;
        }

        public bool HasDied() => HasFlagRaised(EntityFlag.HasDied);
        public bool HasMoved() => HasFlagRaised(EntityFlag.HasMoved);
        public bool HasAttacked() => HasFlagRaised(EntityFlag.HasAttacked);
        public bool HasUsedAbility() => HasFlagRaised(EntityFlag.HasUsedAbility);
        bool HasFlagRaised(EntityFlag flag) => (CurrentFlags & flag) != 0;
        
        public void RaiseFlag(EntityFlag flag)
        {
            CurrentFlags |= flag;
            Entity.NotifyUpdate();
        }
        public void ResetFlag(EntityFlag flag)
        {
            CurrentFlags &= ~flag;
            Entity.NotifyUpdate();
        }
    }
}