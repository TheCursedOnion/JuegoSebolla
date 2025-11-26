using CursedOnion.Game.Cameras;
using CursedOnion.Locators;
using Reflex.Extensions;
using UnityEngine;

namespace CursedOnion.Game.Entity.Components
{
    public class EntityComponent
    {
        protected EntityComponentController AssignedController;
        protected SimpleEntity AssignedEntity;
        protected Transform EntityTransform => AssignedEntity.transform;
        [SerializeField] protected ActionFlag UsedFlags = ActionFlag.None;
        
        public virtual EntityComponent ConfigureComponent(EntityComponentController controller)
        {
            AssignedController = controller;
            AssignedEntity = controller.GetAssignedEntity();
            return this;
        }
        
        protected void RotateEntity(GlobalCamera camera, Transform transform, Vector3 direction)
        {
            float degrees = camera.GetCameraPanAngles();
            direction = Quaternion.AngleAxis(-degrees, Vector3.up) * direction;

            if (direction.x > 0.01f)
            {
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x) * -1f;
                transform.localScale = scale;
            }
            else if (direction.x < -0.01f)
            {
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x);
                transform.localScale = scale;
            }
        }
    }
}