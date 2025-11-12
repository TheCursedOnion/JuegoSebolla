using CursedOnion.Game.Entity.Components;
using NUnit.Framework;
using UnityEngine;

namespace CursedOnion.Game.Entity
{
    [System.Serializable]
    public class EntityComponentController
    {
        [SerializeReference, SubclassSelector] public MoveEntityComponent MoveEntityComponent = new();
        [SerializeReference, SubclassSelector] public AttackEntityComponent AttackEntityComponent = new();
        [SerializeReference, SubclassSelector] public SpecialAbilityComponent AbilityEntityComponent = new();
        public static EntityComponentController Default => new EntityComponentController();

        public EntityComponentController Clone()
        {
            var clone = new EntityComponentController();
            clone.MoveEntityComponent = this.MoveEntityComponent;
            clone.AttackEntityComponent = this.AttackEntityComponent;
            clone.AbilityEntityComponent = this.AbilityEntityComponent;
            
            return clone;
        }
        public virtual EntityComponentController Initialize(SimpleEntity entity)
        {
            MoveEntityComponent?.ConfigureComponent(entity);
            AttackEntityComponent?.ConfigureComponent(entity);
            AbilityEntityComponent?.ConfigureComponent(entity);
            return this;
        }
        public virtual void ProcessTurn(SimpleEntity entity)
        {
            
        }
    }
}
