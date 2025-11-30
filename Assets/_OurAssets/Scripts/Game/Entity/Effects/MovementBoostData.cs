using UnityEngine;

namespace CursedOnion.Game.Entity.Effects
{
    [CreateAssetMenu(fileName = "MovementBoost Effect Data", menuName = "Game/Entity/MovementBoost Effect")]
    public class MovementBoostData : EffectData
    {
        public override StatusEffect CreateInstance(int customDuration = -1, float customMagnitude = -1f)
        {
            int duration = customDuration == -1 ? BaseDuration : customDuration;
            float magnitude = Mathf.Approximately(customMagnitude, -1f) ? BaseMagnitude : customMagnitude;
            return new MovementBoostEffect(this, duration, magnitude);
        }
    }
}