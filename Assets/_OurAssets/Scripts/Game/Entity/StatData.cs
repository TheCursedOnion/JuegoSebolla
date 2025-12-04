using System;
using System.Collections.Generic;
using CursedOnion.Game.Miscellaneous;
using CursedOnion.Game.Modes.General.Animations;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;
using static CursedOnion.Game.Modes.General.Animations.LayeredEntity;
using Random = UnityEngine.Random;

namespace CursedOnion.Game.Entity
{
    [CreateAssetMenu(fileName = "New Stat Data", menuName = "Game/Entity/Stats Data")]
    public class StatData : ScriptableObject
    {
        [Serializable]
        public struct StatRange
        {
            [MinMaxSlider(0, byte.MaxValue)]
            public Vector2Int Range;

            public int RandomValue => Random.Range(Range.x, Range.y + 1);
            public override string ToString() => $"{Range.x}-{Range.y}";
        }
        
        [HorizontalLine(height: 1f, color: EColor.Gray)]
        [Header("Entity Properties")]
        public string EntityName;
        public string EntityNameKey;

        public StatRange Hp;
        public StatRange Attack;
        public StatRange Defense;
        public StatRange Initiative;

        public int Movement;
        public int Price;
        
        [HorizontalLine(height: 1f, color: EColor.Gray)]
        [Header("Entity UI")]
        public GameObject CharacterUI;
        
        [HorizontalLine(height: 1f, color: EColor.Gray)]
        [Header("Entity Components")]
        [SerializeReference, SubclassSelector] public SpecialAbility SpecialAbility;
        public EntityComponents EntityComponents;
        
        [HorizontalLine(height: 1f, color: EColor.Gray)]
        [Header("Entity Visuals")]
        public Sprite InspectorSprite;
        public List<AnimationLayerGroup> AnimationLayers;
        
        
        [HorizontalLine(height: 1f, color: EColor.Gray)]
        [Header("Entity Extras")]
        public TalkData TalkData;
        
        public int GetPrice() => Price;
        public int GetMovement() => Movement;
        public int GetRandomHP() => Hp.RandomValue;
        public int GetRandomAttack() => Attack.RandomValue;
        public int GetRandomDefense() => Defense.RandomValue;
        public int GetRandomInitiative() => Initiative.RandomValue;
    }
}
