using UnityEngine;

namespace CursedOnion.Game.Entity.Effects
{
    [CreateAssetMenu(fileName = "Confusion Effect Data", menuName = "Game/Entity/Confusion Effect")]
    public class ConfusionData : EffectData
    {
        public override StatusEffect CreateInstance(int customDuration = -1, float customMagnitude = -1f)
        {
            int duration = customDuration == -1 ? BaseDuration : customDuration;
            float magnitude = Mathf.Approximately(customMagnitude, -1f) ? BaseMagnitude : customMagnitude;
            return new ConfusionEffect(this, duration, magnitude);
        }
    }
}