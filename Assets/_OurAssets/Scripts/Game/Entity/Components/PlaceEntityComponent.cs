using CursedOnion.Helpers;
using UnityEngine;

namespace CursedOnion.Game.Entity.Components
{
    [System.Serializable]
    public class PlaceEntityComponent : EntityComponent
    {
        [SerializeField] private DirectionFlag blockEntryDirectionsOnPlacement;
        public void PlaceEntity()
        {
            var grid = AssignedEntity.Grid;
            var tile = grid.GetTileAtWorldPosition(AssignedEntity.transform.position);
            tile.PlaceEntity(AssignedEntity, blockEntryDirectionsOnPlacement);
        }
        public void RemoveEntity()
        {
            if(AssignedEntity == null) return;
            var grid = AssignedEntity.Grid;
            grid.GetTileAtWorldPosition(AssignedEntity.transform.position).RemoveEntity(~blockEntryDirectionsOnPlacement);
        }
    }
}