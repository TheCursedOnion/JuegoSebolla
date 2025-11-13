using UnityEngine;

namespace CursedOnion.Game.Entity
{
    public class PlayerUnitController : EntityComponentController
    {
        [SerializeField] protected MeshRenderer turnIndicator;
        public override void Initialize(SimpleEntity entity, EntityComponents components)
        {
            base.Initialize(entity, components);
            
            AssignedEntity.LevelEvents.OnEntitySelected += CheckSelectedEntity;
            AssignedEntity.LevelEvents.OnNoEntitySelected += UnselectEntity;
            turnIndicator = transform.GetChild(0).gameObject.GetComponent<MeshRenderer>();
            turnIndicator.enabled = false;
        }
        protected override void ProcessEntityUpdate(SimpleEntity entity)
        {
            if(entity is not Unit unit) return;

            bool enableTurnIndicator = !unit.GetFlags().HasSpentAllActions();
            turnIndicator.enabled = enableTurnIndicator;
        }
        protected void CheckSelectedEntity(SimpleEntity entity)
        {
            bool entitySelected = entity == AssignedEntity;

            turnIndicator.material.color = entitySelected ? Color.green : Color.gray;
        }
        protected void UnselectEntity()
        {
            CheckSelectedEntity(null);
        }
        public override void ProcessTurn()
        {
            base.ProcessTurn();
            turnIndicator.enabled = true;
        }
        protected override void EndTurn()
        {
            base.EndTurn();
            turnIndicator.enabled = false;
            UnselectEntity();
        }

        public override void Dispose()
        {
            base.Dispose();
            AssignedEntity.LevelEvents.OnEntitySelected -= CheckSelectedEntity;
            AssignedEntity.LevelEvents.OnNoEntitySelected -= UnselectEntity;
        }
    }
}
