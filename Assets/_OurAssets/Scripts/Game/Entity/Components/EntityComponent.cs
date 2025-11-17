using UnityEngine;

namespace CursedOnion.Game.Entity.Components
{
    public class EntityComponent
    {
        protected EntityComponentController AssignedController;
        protected SimpleEntity AssignedEntity;
        protected Transform EntityTransform => AssignedEntity.transform;
        [SerializeField] protected EntityFlag UsedFlags = EntityFlag.None;
        
        public virtual EntityComponent ConfigureComponent(EntityComponentController controller)
        {
            AssignedController = controller;
            AssignedEntity = controller.GetAssignedEntity();
            return this;
        }
    }
}