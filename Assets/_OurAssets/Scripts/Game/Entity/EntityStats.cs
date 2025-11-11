using System.Collections.Generic;
using UnityEngine;
using static CursedOnion.Game.Modes.General.Animations.LayeredEntity;

namespace CursedOnion.Game.Entity
{
    [System.Serializable]
    public class EntityStats
    {
        public int CurrentHealthStat;
        public int MaxHealthStat;
        public virtual void SetStats(EntityData data)
        {
            CurrentHealthStat = MaxHealthStat = data.GetRandomHP();
        }
    }
    
    [System.Serializable]
    public class ExtendedEntityStats : EntityStats
    {
        public int AttackStat;
        public int DefenseStat;
        public int InitiativeStat;
        public int MovementStat;
        public int PriceStat;
        public SpecialAbility SpecialAbilityType;

        public List<AnimationLayer>[] AnimationLayers;
        public override void SetStats(EntityData data)
        {
            CurrentHealthStat = MaxHealthStat = data.GetRandomHP();
            AttackStat = data.GetRandomAttack();
            DefenseStat = data.GetRandomDefense();
            InitiativeStat = data.GetRandomInitiative();
            MovementStat = data.GetMovement();
            PriceStat = data.GetPrice();
            SpecialAbilityType = data.GetSpecialAbility();
            AnimationLayers = data.GetAnimationLayers();
        }
        
    }
}