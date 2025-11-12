using System;
using System.Collections.Generic;
using CursedOnion.Game.Modes.General.Animations;
using NaughtyAttributes;
using UnityEngine;
using static CursedOnion.Game.Modes.General.Animations.LayeredEntity;
using Random = UnityEngine.Random;

namespace CursedOnion.Game.Entity
{
    [CreateAssetMenu(fileName = "New Entity Data", menuName = "Game/Entity/Entity Data")]

    public class EntityData : ScriptableObject
    {
        const string EntityProperties = "Entity Properties";
        const string EntityUI = "Entity UI";
        const string EntityExtras = "Entity Extras";
        const string EntityController = "Entity Controller";
        
        [SerializeField, BoxGroup(EntityProperties)] private string entityName;

        [SerializeField, MinMaxSlider(0, Byte.MaxValue), BoxGroup(EntityProperties)] private Vector2Int hpRange;
        [SerializeField, MinMaxSlider(0, Byte.MaxValue), BoxGroup(EntityProperties)] private Vector2Int attackRange;
        [SerializeField, MinMaxSlider(0, Byte.MaxValue), BoxGroup(EntityProperties)] private Vector2Int defenseRange;
        [SerializeField, MinMaxSlider(0, Byte.MaxValue), BoxGroup(EntityProperties)] private Vector2Int initiativeRange;
        [SerializeField, BoxGroup(EntityProperties)] private int movement;
        [SerializeField, BoxGroup(EntityProperties)] private int price;

        [SerializeField, BoxGroup(EntityUI)] private GameObject characterUI;

        [SubclassSelector, SerializeReference, BoxGroup(EntityExtras)] private SpecialAbility specialAbility;
        [SerializeField, BoxGroup(EntityExtras)] private List<AnimationLayerGroup> animationLayers;
        
        [SerializeField, BoxGroup(EntityController)] private EntityComponentController entityComponentController;
        public EntityComponentController GetEntityController() => entityComponentController;
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
        public List<AnimationLayerGroup> GetAnimationLayers()
        {
            return animationLayers;
        }
    }
}
