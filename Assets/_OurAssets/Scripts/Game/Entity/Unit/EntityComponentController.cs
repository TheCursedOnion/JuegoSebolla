using CursedOnion.Game.Entity.Components;
using NUnit.Framework;
using UnityEngine;

namespace CursedOnion.Game.Entity
{
    [System.Serializable]
    public class EntityComponentController
    {
        [SerializeReference, SubclassSelector] public PlaceEntityComponent PlaceEntityComponent = new();
        [SerializeReference, SubclassSelector] public MoveEntityComponent MoveEntityComponent = new();
        [SerializeReference, SubclassSelector] public AttackEntityComponent AttackEntityComponent = new();
        [SerializeReference, SubclassSelector] public SpecialAbilityComponent AbilityEntityComponent = new();
        public static EntityComponentController Default => new EntityComponentController();

        public EntityComponentController Clone()
        {
            var clone = new EntityComponentController();
            
            clone.PlaceEntityComponent = this.PlaceEntityComponent;
            
            clone.MoveEntityComponent = this.MoveEntityComponent;
            clone.AttackEntityComponent = this.AttackEntityComponent;
            clone.AbilityEntityComponent = this.AbilityEntityComponent;
            
            return clone;
        }
        public virtual EntityComponentController Initialize(SimpleEntity entity)
        {
            PlaceEntityComponent?.ConfigureComponent(entity);
            MoveEntityComponent?.ConfigureComponent(entity);
            AttackEntityComponent?.ConfigureComponent(entity);
            AbilityEntityComponent?.ConfigureComponent(entity);
            
            var turnSystem = entity.LevelManager.GetTurnSystem();
            //turnSystem.AddUnit(this);
            //turnSystem.OnTurnStart += HandleTurnStart;
            //turnSystem.OnTurnEnd += HandleTurnEnd;
            
            return this;
        }
        public virtual void ProcessTurn()
        {
            
        }
    }
}
