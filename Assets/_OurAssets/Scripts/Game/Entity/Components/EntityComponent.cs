using UnityEngine;

namespace CursedOnion.Game.Entity.Components
{
    public class EntityComponent
    {
        protected SimpleEntity AssignedEntity;
        protected Transform EntityTransform => AssignedEntity.transform;
        [SerializeField] protected EntityFlag UsedFlags = EntityFlag.None;

        public virtual void ConfigureComponent(SimpleEntity assignedEntity)
        {
            AssignedEntity = assignedEntity;
        }
    }
}