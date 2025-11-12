using CursedOnion.Game.Entity.Components;
using NUnit.Framework;
using UnityEngine;

namespace CursedOnion.Game.Entity
{
    public class EntityComponentController : MonoBehaviour
    {
        [SerializeReference, SubclassSelector] public MoveEntityComponent MoveEntityComponent;

        public virtual EntityComponentController Initialize(SimpleEntity entity)
        {
            MoveEntityComponent = new MoveEntityComponent();
            MoveEntityComponent?.ConfigureComponent(entity);
            //TODO: MAS COMPONENTES
            
            return this;
        }
        public virtual void ProcessTurn(SimpleEntity entity)
        {
            
        }
    }
}
