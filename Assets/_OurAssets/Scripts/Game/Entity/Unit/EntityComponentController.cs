using System;
using CursedOnion.Game.Entity.Components;
using NaughtyAttributes;
using NUnit.Framework;
using UnityEngine;

namespace CursedOnion.Game.Entity
{
    [System.Serializable]
    public struct EntityComponents
    {
        public static EntityComponents Default => new EntityComponents();
        
        [SerializeReference, SubclassSelector] public PlaceEntityComponent PlaceEntityComponent;
        [SerializeReference, SubclassSelector] public MoveEntityComponent MoveEntityComponent;
        [SerializeReference, SubclassSelector] public AttackEntityComponent AttackEntityComponent;
        [SerializeReference, SubclassSelector] public SpecialAbilityComponent AbilityEntityComponent;

    }
    public class EntityComponentController : MonoBehaviour, IDisposable
    {
        
        protected SimpleEntity AssignedEntity;
        [SerializeField, ReadOnly] public PlaceEntityComponent PlaceEntityComponent;
        [SerializeField, ReadOnly] public MoveEntityComponent MoveEntityComponent;
        [SerializeField, ReadOnly] public AttackEntityComponent AttackEntityComponent;
        [SerializeField, ReadOnly] public SpecialAbilityComponent AbilityEntityComponent;
        public virtual void Initialize(SimpleEntity entity, EntityComponents components)
        {
            AssignedEntity = entity;
            PlaceEntityComponent = components.PlaceEntityComponent;
            MoveEntityComponent = components.MoveEntityComponent;
            AttackEntityComponent = components.AttackEntityComponent;
            AbilityEntityComponent = components.AbilityEntityComponent;
            
            PlaceEntityComponent?.ConfigureComponent(entity);
            MoveEntityComponent?.ConfigureComponent(entity);
            AttackEntityComponent?.ConfigureComponent(entity);
            AbilityEntityComponent?.ConfigureComponent(entity);
            
            AssignedEntity.OnEntityUpdate += ProcessEntityUpdate;
            RegisterEntityForTurn();
        }
        void RegisterEntityForTurn()
        {
            if (AssignedEntity is Unit unit)
            {
                var levelEvents = unit.LevelManager.LevelEvents;
                
                levelEvents.RegisterUnitForTurn(unit);
                levelEvents.OnTurnEnded += EndTurn;
            }
        }

        protected virtual void ProcessEntityUpdate(SimpleEntity entity)
        {
            
        }
        public virtual void ProcessTurn()
        {
            AssignedEntity.GetFlags().ResetFlag(EntityFlag.HasMoved);
            AssignedEntity.GetFlags().ResetFlag(EntityFlag.HasAttacked);
            AssignedEntity.GetFlags().ResetFlag(EntityFlag.HasUsedAbility);
        }
        protected virtual void EndTurn()
        {
            AssignedEntity.Grid.ResetPaint();
        }
        
        public virtual void Dispose()
        {
            PlaceEntityComponent?.Remove();
            
            AssignedEntity.OnEntityUpdate -= ProcessEntityUpdate;
            if (AssignedEntity is Unit unit)
            {
                unit.LevelManager.LevelEvents.OnTurnEnded -= EndTurn;
            }
        }
        
    }
}
