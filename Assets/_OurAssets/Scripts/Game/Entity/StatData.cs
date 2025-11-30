using System;
using System.Collections.Generic;
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
        private const string EntityProperties = "Entity Properties";
        private const string EntityUI = "Entity UI";
        private const string EntityExtras = "Entity Extras";
        private const string EntityController = "Entity Controller";
        private const string VisualData = "Visual Data";

        [Serializable]
        public struct StatRange
        {
            [MinMaxSlider(0, byte.MaxValue)]
            public Vector2Int Range;

            public int RandomValue => Random.Range(Range.x, Range.y + 1);
            public override string ToString() => $"{Range.x}-{Range.y}";
        }
        
        [BoxGroup(EntityProperties)] public string EntityName;
        [BoxGroup(EntityProperties)] public string EntityNameKey;

        [BoxGroup(EntityProperties)] public StatRange Hp;
        [BoxGroup(EntityProperties)] public StatRange Attack;
        [BoxGroup(EntityProperties)] public StatRange Defense;
        [BoxGroup(EntityProperties)] public StatRange Initiative;

        [BoxGroup(EntityProperties)] public int Movement;
        [BoxGroup(EntityProperties)] public int Price;
        
        [BoxGroup(EntityUI)] public GameObject CharacterUI;
        
        [BoxGroup(EntityExtras), SerializeReference, SubclassSelector] public SpecialAbility SpecialAbility;

        [BoxGroup(EntityExtras)] public List<AnimationLayerGroup> AnimationLayers;
        
        [BoxGroup(EntityController)] public EntityComponents EntityComponents;
        
        [BoxGroup(VisualData)] public Sprite InspectorSprite;
        public GameObject UI => CharacterUI;
        public int GetPrice() => Price;
        public int GetMovement() => Movement;

        public int GetRandomHP() => Hp.RandomValue;
        public int GetRandomAttack() => Attack.RandomValue;
        public int GetRandomDefense() => Defense.RandomValue;
        public int GetRandomInitiative() => Initiative.RandomValue;
    }
}
