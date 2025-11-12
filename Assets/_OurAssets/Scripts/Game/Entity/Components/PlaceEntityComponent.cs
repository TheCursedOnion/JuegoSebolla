using CursedOnion.Helpers;
using UnityEngine;

namespace CursedOnion.Game.Entity.Components
{
    [System.Serializable]
    public class PlaceEntityComponent : EntityComponent
    {
        [SerializeField] private DirectionFlag blockEntryDirectionsOnPlacement;
        
        public override void ConfigureComponent(SimpleEntity assignedEntity)
        {
            AssignedEntity = assignedEntity;
            Place();
        }
        public void Place()
        {
            var grid = AssignedEntity.Grid;
            grid.GetTileAtWorldPosition(AssignedEntity.transform.position).PlaceEntity(AssignedEntity, blockEntryDirectionsOnPlacement);
        }
        public void Remove()
        {
            var grid = AssignedEntity.Grid;
            grid.GetTileAtWorldPosition(AssignedEntity.transform.position).RemoveEntity(~blockEntryDirectionsOnPlacement);
        }
    }
}