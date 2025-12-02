using System;
using UnityEngine;

namespace CursedOnion.Game.Entity.Effects
{
    public class StatusEffect
    {
        public event Action OnStart;
        public event Action OnEnd;
        
        protected EffectData Data;
        protected int Duration;
        protected int RemainingDuration;
        protected readonly float Magnitude;

        #region Getters
        public EffectData GetData() => Data;
        
        public int GetRemainingDuration() => RemainingDuration;
        public int GetDuration() => Duration;
        
        #endregion
        public StatusEffect(EffectData data, int duration, float magnitude)
        {
            Data = data;
            this.Duration = RemainingDuration = duration;
            this.Magnitude = magnitude;
        }
        protected void RaiseStart() => OnStart?.Invoke();
        protected void RaiseEnd() => OnEnd?.Invoke();
        
        public virtual void ApplyOn(StatusHandler status)
        {
            RaiseStart();
        }
        public virtual void Remove(StatusHandler status) => RaiseEnd();

        public virtual void ResetDuration(int duration)
        {
            int newDuration = Mathf.Max(Duration, duration);
            Duration = RemainingDuration = newDuration;
        }
        
        public bool UpdateEffect(StatusHandler status)
        {
            bool hasEnded = RemainingDuration-- <= 0;
            if (hasEnded) Remove(status);
            
            return hasEnded;
        }
    }

    public class ConfusionEffect : StatusEffect
    {
        public ConfusionEffect(EffectData data, int duration, float magnitude) : base(data, duration, magnitude) {}
    }

    public class AttackBoostEffect : StatusEffect
    {
        public AttackBoostEffect(EffectData data, int duration, float magnitude) : base(data, duration, magnitude) {}
        
        public override void ApplyOn(StatusHandler status)
        {
            status.AttackMultiplier = Magnitude;
            RaiseStart();
        }

        public override void Remove(StatusHandler status)
        {
            status.AttackMultiplier = 1;
            RaiseEnd();
        }
    }
    
    public class MovementBoostEffect : StatusEffect
    {
        public MovementBoostEffect(EffectData data, int duration, float magnitude) : base(data, duration, magnitude) {}
        
        public override void ApplyOn(StatusHandler status)
        {
            status.AdditionalMovement = (int)Magnitude;
            RaiseStart();
        }
        public override void Remove(StatusHandler status)
        {
            status.AdditionalMovement = 0;
            RaiseEnd();
        }
    }
    
    public class HealthBoostEffect : StatusEffect
    {
        public HealthBoostEffect(EffectData data, int duration, float magnitude) : base(data, duration, magnitude) {}
        
        public override void ApplyOn(StatusHandler status)
        {
            status.SetAdditionalHP(Magnitude);
            RaiseStart();
        }
        public override void Remove(StatusHandler status)
        {
            status.SetAdditionalHP(0);
            RaiseEnd();
        }
    }
}