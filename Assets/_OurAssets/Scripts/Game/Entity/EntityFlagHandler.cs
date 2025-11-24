using System;

namespace CursedOnion.Game.Entity
{
    public class EntityFlagHandler<T> where T : Enum
    {
        protected T currentFlags;
        protected SimpleEntity entityOwner;
        private static int ToInt(T value) => Convert.ToInt32(value);
        private static T ToEnum(int value) => (T)Enum.ToObject(typeof(T), value);
        
        public EntityFlagHandler(SimpleEntity entityOwner)
        {
            currentFlags = default;
            this.entityOwner = entityOwner;
        }
        public bool HasFlagRaised(T flag)
        {
            int current = ToInt(currentFlags);
            int raised = ToInt(flag);
            return (current & raised) != 0;
        }

        public void RaiseFlag(T flag)
        {
            int current = ToInt(currentFlags);
            int rising = ToInt(flag);
            currentFlags = ToEnum(current | rising);
            entityOwner.NotifyActionUpdate();
        }
        public void ResetFlag(T flag)
        {
            int current = ToInt(currentFlags);
            int resetting = ToInt(flag);
            currentFlags = ToEnum(current & ~resetting);
            entityOwner.NotifyActionUpdate();
        }
    }
}