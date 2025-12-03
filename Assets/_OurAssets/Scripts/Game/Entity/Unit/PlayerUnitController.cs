using CursedOnion.Extensions;
using CursedOnion.Game.Systems.Grid;
using UnityEngine;

namespace CursedOnion.Game.Entity
{
    public class PlayerUnitController : EntityComponentController
    {
        [SerializeField] protected SpriteRenderer turnIndicator;
        [SerializeField] private Color beingInspectedColor = Color.green;
        [SerializeField] private Color notBeingInspectedColor = new Color(1, 0.87f, 0 , 0.5f);
        bool hasTurn = false;
        public override void Initialize(SimpleEntity entity, EntityComponents components)
        {
            base.Initialize(entity, components);
            
            AssignedEntity.LevelEvents.OnEntitySelected += CheckSelectedEntity;
            AssignedEntity.LevelEvents.OnNoEntitySelected += UnselectEntity;
            
            turnIndicator = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
            turnIndicator.enabled = false;
        }
        protected override void ProcessEntityUpdate(SimpleEntity entity)
        {
            if(entity is not Unit unit) return;

            bool enableTurnIndicator = !unit.ActionHandler.HasSpentAllActions();
            //turnIndicator.enabled = enableTurnIndicator;
        }
        protected void CheckSelectedEntity(SimpleEntity entity)
        {
            if(!hasTurn) return;
            
            bool entitySelected = entity == AssignedEntity;
            turnIndicator.enabled = true;
            
            Color color = entitySelected ? beingInspectedColor : notBeingInspectedColor;
            turnIndicator.color = color;
        }
        protected void UnselectEntity()
        {
            CheckSelectedEntity(null);
        }
        public override void ProcessTurn()
        {
            base.ProcessTurn();
            hasTurn = true;
            turnIndicator.enabled = true;
        }
        protected override void EndTurn()
        {
            base.EndTurn();
            
            hasTurn = false;
            AssignedEntity.ActionHandler.ResetAllActions();
            turnIndicator.enabled = false;
        }

        public override void Dispose()
        {
            base.Dispose();
            AssignedEntity.LevelEvents.OnEntitySelected -= CheckSelectedEntity;
            AssignedEntity.LevelEvents.OnNoEntitySelected -= UnselectEntity;
        }
    }
}
