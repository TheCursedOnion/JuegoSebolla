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
        
        [SerializeField] protected SimpleEntity AssignedEntity;
        [SerializeField, ReadOnly] protected PlaceEntityComponent PlaceEntityComponent;
        [SerializeField, ReadOnly] protected MoveEntityComponent MoveEntityComponent;
        [SerializeField, ReadOnly] protected AttackEntityComponent AttackEntityComponent;
        [SerializeField, ReadOnly] protected SpecialAbilityComponent AbilityEntityComponent;

        public PlaceEntityComponent PlaceComponent => GetEntityComponent<PlaceEntityComponent>();
        public MoveEntityComponent MoveComponent => GetEntityComponent<MoveEntityComponent>();
        public AttackEntityComponent AttackComponent => GetEntityComponent<AttackEntityComponent>();
        public SpecialAbilityComponent AbilityComponent => GetEntityComponent<SpecialAbilityComponent>();
        
        public SimpleEntity GetAssignedEntity() => AssignedEntity;
        public T GetEntityComponent<T>() where T : EntityComponent
        {
            EntityComponent component = typeof(T) switch
            {
                _ when typeof(T) == typeof(PlaceEntityComponent) => PlaceEntityComponent,
                _ when typeof(T) == typeof(MoveEntityComponent) => MoveEntityComponent,
                _ when typeof(T) == typeof(AttackEntityComponent) => AttackEntityComponent,
                _ when typeof(T) == typeof(SpecialAbilityComponent) => AbilityEntityComponent,
                _ => null
            };

            return component?.ConfigureComponent(this) as T;
        }
        public virtual void Initialize(SimpleEntity entity, EntityComponents components)
        {
            AssignedEntity = entity;
            PlaceEntityComponent = components.PlaceEntityComponent;
            MoveEntityComponent = components.MoveEntityComponent;
            AttackEntityComponent = components.AttackEntityComponent;
            AbilityEntityComponent = components.AbilityEntityComponent;
            
            
            
            AssignedEntity.OnEntityUpdate += ProcessEntityUpdate;
            RegisterEntityForTurn();
        }
        
        #region Turn Handling
        void RegisterEntityForTurn()
        {
            if (AssignedEntity is Unit unit)
            {
                var levelEvents = unit.LevelManager.LevelEvents;
                levelEvents.RegisterUnitForTurn(unit);
                levelEvents.OnTurnEnded += EndTurn;
            }
        }
        public virtual void ProcessTurn()
        {
            AssignedEntity.UpdateStatusEffects();
            AssignedEntity.ActionHandler.ResetAllActions();
        }
        protected virtual void EndTurn()
        {
            AssignedEntity.Grid.ResetPaint();
        }
        #endregion
        protected virtual void ProcessEntityUpdate(SimpleEntity entity)
        {
            
        }
        public virtual void Dispose()
        {
            GetEntityComponent<PlaceEntityComponent>().RemoveEntity();

            AssignedEntity.OnEntityUpdate -= ProcessEntityUpdate;
            if (AssignedEntity is Unit unit)
            {
                var levelEvents = unit.LevelManager.LevelEvents;
                levelEvents.UnregisterUnitForTurn(unit);
                unit.LevelManager.LevelEvents.OnTurnEnded -= EndTurn;
            }
        }
        
    }
}
