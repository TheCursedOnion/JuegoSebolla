using UnityEngine;

namespace CursedOnion.Game.Entity.Components
{
    public class EntityComponent
    {
        protected SimpleEntity AssignedEntity;
        protected Transform EntityTransform => AssignedEntity.transform;

        public virtual void ConfigureComponent(SimpleEntity assignedEntity)
        {
            AssignedEntity = assignedEntity;
        }

        public virtual void Cancel()
        {
            
        }
    }
}