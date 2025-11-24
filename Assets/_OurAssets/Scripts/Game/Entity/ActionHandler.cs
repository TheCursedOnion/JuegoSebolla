using System;

namespace CursedOnion.Game.Entity
{
    [Flags]
    public enum ActionFlag
    {
        None        = 0,
        
        HasDied     = 1 << 0,
        HasMoved    = 1 << 1,
        HasAttacked = 1 << 2,
        HasUsedAbility = 1 << 3,
        IsNotIdle   = 1 << 4,
    }
    public class ActionHandler : EntityFlagHandler<ActionFlag>
    {
        public ActionHandler(SimpleEntity entityOwner) : base(entityOwner) {}
        public bool HasMoved() => HasFlagRaised(ActionFlag.HasMoved);
        public bool HasAttacked() => HasFlagRaised(ActionFlag.HasAttacked);
        public bool HasUsedAbility() => HasFlagRaised(ActionFlag.HasUsedAbility);
        
        public bool IsNotIdle() => HasFlagRaised(ActionFlag.IsNotIdle);
        public bool HasSpentAllActions() => HasAttacked() && HasUsedAbility() && HasMoved();
        
        public void ResetAllActions() => ResetFlag(ActionFlag.HasAttacked | ActionFlag.HasUsedAbility | ActionFlag.HasMoved);
    }
}