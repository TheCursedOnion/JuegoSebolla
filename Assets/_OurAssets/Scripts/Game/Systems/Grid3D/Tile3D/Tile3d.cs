using CursedOnion.Extensions;
using CursedOnion.Game.Systems.Grid.Scriptable;
using CursedOnion.Helpers;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CursedOnion.Game.Systems.Grid
{
    
    [System.Serializable]
    public class Tile3d
    {
        [SerializeField] Tile3dDescriptor descriptor = Tile3dDescriptor.Default;
        [SerializeField] Mesh gridMesh;
        [SerializeField] IntRange correspondingVerticesInMesh = new(-1, -1);
        IEntity containedEntity;
        public static Tile3d Default
        {
            get
            {
                var defaultTile = new Tile3d
                {
                    descriptor = Tile3dDescriptor.Default,
                    gridMesh = null,
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
        public void SetContainedEntity(IEntity newContainedEntity) => containedEntity = newContainedEntity;
        
        public void SetTileDescriptor(Tile3dDescriptor tileDescriptor)
        {
            this.descriptor = tileDescriptor;
        }
        public void SetTileMeshProperties(Mesh combinedGridMesh, IntRange verticesInMesh)
        {
            gridMesh = combinedGridMesh;
            correspondingVerticesInMesh = verticesInMesh;
        }
        #endregion
        
        public void Paint(Color color)
        {
            if(gridMesh != null)
                gridMesh.Color32Vertices(correspondingVerticesInMesh, color);
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
            this.gridMesh = tile.gridMesh;
            this.correspondingVerticesInMesh = tile.correspondingVerticesInMesh;
            this.containedEntity = tile.containedEntity;
        }
        public void DebugTile()
        {
            Debug.Log($"Tile Debug: {descriptor.Id}; [{correspondingVerticesInMesh.Start}, {correspondingVerticesInMesh.End}]");
        }
        
    }
}