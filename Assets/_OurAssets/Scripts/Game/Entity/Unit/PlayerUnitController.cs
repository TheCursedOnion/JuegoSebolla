using CursedOnion.Extensions;
using CursedOnion.Game.Modes.General.Animations;
using CursedOnion.Game.Systems.Grid;
using UnityEngine;

namespace CursedOnion.Game.Entity
{
    public class PlayerUnitController : EntityComponentController
    {
        [SerializeField] protected SpriteRenderer turnIndicator;
        [SerializeField] private Color beingInspectedColor = Color.green;
        [SerializeField] private Color notBeingInspectedColor = new Color(1, 0.87f, 0 , 0.5f);

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
            if(!AssignedEntity.HasTurn) return;
            
            bool entitySelected = entity == AssignedEntity;
            
            AssignedEntity.BeingInspected = entitySelected;
            
            Color color = entitySelected ? beingInspectedColor : notBeingInspectedColor;
            turnIndicator.color = color;

            if (AssignedEntity.TryGetLayeredEntity(out LayeredEntity layeredEntity))
            {
                layeredEntity.PlayAnimation(entitySelected ? "think" : "idle");
            }
        }
        protected void UnselectEntity()
        {
            CheckSelectedEntity(null);
        }
        public override void ProcessTurn()
        {
            AssignedEntity.HasTurn = true;
            
            if (AssignedEntity.StatusHandler.HasConfusionEffect())
            {
                EndTurn();
                AssignedEntity.ActionHandler.RaiseAllActions();
                AssignedEntity.UpdateStatusEffects();
                return;
            }
            
            base.ProcessTurn();
            turnIndicator.enabled = true;
        }
        protected override void EndTurn()
        {
            if(!AssignedEntity.HasTurn) return;
            
            base.EndTurn();
            
            AssignedEntity.HasTurn = false;
            AssignedEntity.BeingInspected = false;
            
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
