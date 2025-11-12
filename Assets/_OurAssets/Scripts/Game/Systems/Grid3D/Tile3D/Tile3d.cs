using System.Collections.Generic;
using CursedOnion.Extensions;
using CursedOnion.Game.Systems.Grid.Scriptable;
using CursedOnion.Game;
using CursedOnion.Game.Entity;
using CursedOnion.Helpers;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CursedOnion.Game.Systems.Grid
{
    [System.Serializable]
    public class Tile3d
    {
        [SerializeField] Tile3dDescriptor descriptor = Tile3dDescriptor.Default;
        [SerializeField] TileAttributes attributes;
        
        [SerializeField] DirectionFlag transformedExitDirections = DirectionFlag.None;
        [SerializeField] DirectionFlag transformedEntryDirections = DirectionFlag.None;
        
        [SerializeField] DirectionFlag blockedEntryDirections = DirectionFlag.None;
        SimpleEntity containedEntity;
        public static Tile3d Default => new(Tile3dDescriptor.Default, TileAttributes.Default);

        public Tile3d(Tile3dDescriptor tileDescriptor, TileAttributes tileAttributes)
        {
            Configure(tileDescriptor, tileAttributes);
        }
        
        #region Getters & Setters
        public Tile3dDescriptor GetTileDescriptor() => descriptor;
        public SimpleEntity GetContainedEntity() => containedEntity;
        public void PlaceEntity(SimpleEntity entity, DirectionFlag blockDirections = DirectionFlag.None)
        {
            containedEntity = entity;
            blockedEntryDirections = blockDirections;
        }
        public void RemoveEntity(DirectionFlag unblockDirections = DirectionFlag.None)
        {
            containedEntity = null;
            blockedEntryDirections &= unblockDirections;
        }
        
        public DirectionFlag GetEntryDirections() => transformedEntryDirections;
        public DirectionFlag GetExitDirections() => transformedExitDirections;
        public List<Vector3> GetExitDirectionVector() => DirectionHelper.GetDirectionVectors(transformedExitDirections);
        
        public DirectionFlag GetBlockedEntryDirections() => blockedEntryDirections;
        public void SetBlockedEntryDirections(DirectionFlag flag) =>  blockedEntryDirections = flag;
        public void BlockEntryDirection(DirectionFlag direction) => blockedEntryDirections |= direction;
        public void UnblockEntryDirection(DirectionFlag direction) => blockedEntryDirections &= ~direction;
        
        public TileAttributes GetTileAttributes() => attributes;
        #endregion
        
        public void Configure(Tile3dDescriptor tileDescriptor, TileAttributes tileAttributes)
        {
            descriptor = tileDescriptor;
            attributes = tileAttributes;
            transformedExitDirections = tileDescriptor.AllowedExitDirections;
            transformedEntryDirections = tileDescriptor.AllowedEntryDirections;
        }

        public void RotateTile(float eulerYRotation)
        {
            DirectionHelper.RotateFlagsAroundYAxis(ref transformedExitDirections, eulerYRotation);
            DirectionHelper.RotateFlagsAroundYAxis(ref transformedEntryDirections, eulerYRotation);
        }
        public Tile3d Clone()
        {
            var clone = Default;
            clone.ReplaceAttributes(this);
            return clone;
        }
        public void ReplaceAttributes(Tile3d tile)
        {
            this.descriptor = tile.descriptor;
            this.attributes = tile.attributes;

            this.containedEntity = tile.containedEntity;
            
            this.blockedEntryDirections = tile.blockedEntryDirections;
            this.transformedEntryDirections = tile.transformedEntryDirections;
            this.transformedExitDirections = tile.transformedExitDirections;
        }
        
        public bool CanBeAccessedFrom(Vector3 direction)
        {
            var flag = DirectionHelper.GetDirectionFlag(direction);
            return CanBeAccessedFrom(flag);
        }
        public bool CanBeAccessedFrom(DirectionFlag direction)
        {
            return (transformedEntryDirections & ~blockedEntryDirections & direction) != 0;
        }
        public bool IsBlocked()
        {
            return (transformedEntryDirections & ~blockedEntryDirections) == 0;
        }
        public bool IsEmptyTile()
        {
            return descriptor.IsAirBlock;
        }
        public bool IsFullTile()
        {
            return descriptor.IsFullBlock;
        }
        public bool IsFluidTile()
        {
            return descriptor.IsFluidBlock;
        }
        
        public void DebugTile()
        {
            //Debug.Log($"Tile Debug: {descriptor.Id}; [{correspondingVerticesInMesh.Start}, {correspondingVerticesInMesh.End}]");
        }
        
    }
}