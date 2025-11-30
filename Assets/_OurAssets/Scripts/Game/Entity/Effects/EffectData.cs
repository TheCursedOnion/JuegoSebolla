using NaughtyAttributes;
using UnityEngine;

namespace CursedOnion.Game.Entity.Effects
{
    public abstract class EffectData: ScriptableObject
    {
        [Header("Visual")]
        public bool HasVisual = false;
        [ShowIf("HasVisual")] public Sprite Icon;
        public string DisplayName;
        
        [Header("Base parameters")]
        public int BaseDuration = 1;
        public float BaseMagnitude = 1f;
        
        public abstract StatusEffect CreateInstance(int customDuration = -1, float customMagnitude = -1f);  
    }
}