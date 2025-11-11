using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using static CursedOnion.Game.Modes.General.Animations.LayeredEntity;
using Random = UnityEngine.Random;

namespace CursedOnion.Game.Entity
{
    [CreateAssetMenu(fileName = "New Entity Data", menuName = "Game/Entity/Entity Data")]

    public class EntityData : ScriptableObject
    {
        [SerializeField] private string entityName;

        [SerializeField, MinMaxSlider(0, Byte.MaxValue)] private Vector2Int hpRange;
        [SerializeField, MinMaxSlider(0, Byte.MaxValue)] private Vector2Int attackRange;
        [SerializeField, MinMaxSlider(0, Byte.MaxValue)] private Vector2Int defenseRange;
        [SerializeField, MinMaxSlider(0, Byte.MaxValue)] private Vector2Int initiativeRange;
        [SerializeField] private int movement;
        [SerializeField] private int price;

        [SerializeField] private GameObject characterUI;

        [SubclassSelector, SerializeReference] private SpecialAbility specialAbility;

        [SerializeField] private List<AnimationLayer>[] animationLayers;

        public string GetName()
        { 
            return entityName;
        }
        public int GetRandomHP()
        {
            return Random.Range(hpRange.x, hpRange.y + 1);
        }
        public int GetRandomInitiative()
        {
            return Random.Range(initiativeRange.x, initiativeRange.y + 1);
        }
        public int GetRandomAttack()
        {
            return Random.Range(attackRange.x, attackRange.y + 1);
        }
        public int GetRandomDefense()
        {
            return Random.Range(defenseRange.x, defenseRange.y + 1);
        }
        public int GetMovement()
        {
            return movement;
        }
        public int GetPrice()
        {
            return price;
        }
        public GameObject GetUI()
        {
            return characterUI;
        }
        public SpecialAbility GetSpecialAbility()
        {
            return specialAbility;
        }
        public List<AnimationLayer>[] GetAnimationLayers()
        {
            return animationLayers;
        }
    }
}
