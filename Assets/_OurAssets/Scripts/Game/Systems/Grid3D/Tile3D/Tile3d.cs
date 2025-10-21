using CursedOnion.Extensions;
using CursedOnion.Game.Systems.Grid.Scriptable;
using CursedOnion.Game;
using CursedOnion.Helpers;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CursedOnion.Game.Systems.Grid
{
    
    [System.Serializable]
    public class Tile3d
    {
        [SerializeField] Tile3dDescriptor descriptor = Tile3dDescriptor.Default;
        [SerializeField] IntRange correspondingVerticesInMesh = new(-1, -1);
        
        [SerializeField] DirectionFlag blockedEntryDirections = DirectionFlag.None;
        IEntity containedEntity;
        public static Tile3d Default
        {
            get
            {
                var defaultTile = new Tile3d
                {
                    descriptor = Tile3dDescriptor.Default,
                    correspondingVerticesInMesh = IntRange.Default,
                    containedEntity = null
                };
                return defaultTile;
            }
        }
        
        #region Getters & Setters
        public Tile3dDescriptor GetTileDescriptor() => descriptor;
        public IntRange CorrespondingVerticesInMesh => correspondingVerticesInMesh;
        
        public IEntity GetContainedEntity() => containedEntity;
        public void SetContainedEntity(IEntity newContainedEntity) 
        { 
            containedEntity = newContainedEntity;
        }
        public void SetTileVertices(IntRange verticesInMesh)
        {
            correspondingVerticesInMesh = verticesInMesh;
        }
        
        //Como es una flag, hay que emplear operaciones de bits :)
        public DirectionFlag GetBlockedEntryDirections() => blockedEntryDirections;
        public void SetBlockedEntryDirections(DirectionFlag flag) =>  blockedEntryDirections = flag;
        public void BlockEntryDirection(DirectionFlag direction) => blockedEntryDirections |= direction;
        public void UnblockEntryDirection(DirectionFlag direction) => blockedEntryDirections &= ~direction;
        
        public void SetTileDescriptor(Tile3dDescriptor tileDescriptor)
        {
            this.descriptor = tileDescriptor;
        }
        #endregion
        
        public void Paint(Mesh mesh, Color color)
        {
            mesh.Color32Vertices(correspondingVerticesInMesh, color);
        }
        public Tile3d Clone()
        {
            var clone = new Tile3d();
            clone.ReplaceAttributes(this);
            return clone;
        }
        public void ReplaceAttributes(Tile3d tile)
        {
            this.descriptor = tile.descriptor;
            this.correspondingVerticesInMesh = tile.correspondingVerticesInMesh;
            this.containedEntity = tile.containedEntity;
            this.blockedEntryDirections = tile.blockedEntryDirections;
        }

        public bool IsEmptyTile()
        {
            return descriptor == null || descriptor.Id == 0;
        }
        public void DebugTile()
        {
            Debug.Log($"Tile Debug: {descriptor.Id}; [{correspondingVerticesInMesh.Start}, {correspondingVerticesInMesh.End}]");
        }
        
    }
}