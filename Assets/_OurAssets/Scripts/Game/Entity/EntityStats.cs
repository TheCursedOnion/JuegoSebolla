using System.Collections.Generic;
using UnityEngine;
using CursedOnion.Game.Modes.General.Animations;

namespace CursedOnion.Game.Entity
{
    [System.Serializable]
    public class EntityStats
    {
        public int CurrentHealthStat;
        public int MaxHealthStat;
        public virtual void SetWithData(StatData data)
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
        public List<AnimationLayerGroup> AnimationLayers;
        public override void SetWithData(StatData data)
        {
            CurrentHealthStat = MaxHealthStat = data.GetRandomHP();
            
            AttackStat = data.GetRandomAttack();
            DefenseStat = data.GetRandomDefense();
            InitiativeStat = data.GetRandomInitiative();
            MovementStat = data.GetMovement();
            PriceStat = data.GetPrice();
            
            SpecialAbilityType = data.SpecialAbility;
            AnimationLayers = data.AnimationLayers;
        }
        
    }
}