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
        private SimpleEntity assignedEntity;
        [SerializeField, ReadOnly] public PlaceEntityComponent PlaceEntityComponent;
        [SerializeField, ReadOnly] public MoveEntityComponent MoveEntityComponent;
        [SerializeField, ReadOnly] public AttackEntityComponent AttackEntityComponent;
        [SerializeField, ReadOnly] public SpecialAbilityComponent AbilityEntityComponent;
        public void Initialize(SimpleEntity entity, EntityComponents components)
        {
            assignedEntity = entity;
            PlaceEntityComponent = components.PlaceEntityComponent;
            MoveEntityComponent = components.MoveEntityComponent;
            AttackEntityComponent = components.AttackEntityComponent;
            AbilityEntityComponent = components.AbilityEntityComponent;
            
            PlaceEntityComponent?.ConfigureComponent(entity);
            MoveEntityComponent?.ConfigureComponent(entity);
            AttackEntityComponent?.ConfigureComponent(entity);
            AbilityEntityComponent?.ConfigureComponent(entity);

            RegisterEntityForTurn();
        }
        
        void RegisterEntityForTurn()
        {
            if (assignedEntity is Unit unit)
            {
                var levelEvents = unit.LevelManager.LevelEvents;
                
                levelEvents.RegisterUnitForTurn(unit);
                levelEvents.OnTurnEnded += EndedTurn;
            }
        }
        public virtual void ProcessTurn()
        {
            assignedEntity.GetFlags().ResetFlag(EntityFlag.HasMoved);
            assignedEntity.GetFlags().ResetFlag(EntityFlag.HasAttacked);
            assignedEntity.GetFlags().ResetFlag(EntityFlag.HasUsedAbility);
        }
        void EndedTurn()
        {
            assignedEntity.Grid.ResetPaint();
        }
        
        public void Dispose()
        {
            PlaceEntityComponent?.Cancel();
            if (assignedEntity is Unit unit)
            {
                unit.LevelManager.LevelEvents.OnTurnEnded -= EndedTurn;
            }
        }
        
    }
}
