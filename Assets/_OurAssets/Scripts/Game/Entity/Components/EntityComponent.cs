using UnityEngine;

namespace CursedOnion.Game.Entity.Components
{
    public class EntityComponent
    {
        protected EntityComponentController AssignedController;
        protected SimpleEntity AssignedEntity;
        protected Transform EntityTransform => AssignedEntity.transform;
        [SerializeField] protected EntityFlag UsedFlags = EntityFlag.None;

        public EntityComponent GetComponent(EntityComponentController controller)
        {
            ConfigureComponent(controller);
            return this;
        }
        public virtual void ConfigureComponent(EntityComponentController controller)
        {
            AssignedController = controller;
            AssignedEntity = controller.GetAssignedEntity();
        }
    }
}