using UnityEngine;

namespace CursedOnion.Game.Entity.Effects
{
    [CreateAssetMenu(fileName = "HealthBoost Effect Data", menuName = "Game/Entity/HealthBoost Effect")]
    public class HealthBoostData : EffectData
    {
        public override StatusEffect CreateInstance(int customDuration = -1, float customMagnitude = -1f)
        {
            int duration = customDuration == -1 ? BaseDuration : customDuration;
            float magnitude = Mathf.Approximately(customMagnitude, -1f) ? BaseMagnitude : customMagnitude;
            return new HealthBoostEffect(this, duration, magnitude);
        }
    }
}