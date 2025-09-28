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
        public static Tile3d Default
        {
            get
            {
                var defaultTile = new Tile3d(null);
                defaultTile.Descriptor = Tile3dDescriptor.Default;
                return defaultTile;
            }
        }
        
        [SerializeField] Mesh gridMesh;
        [SerializeField] IntRange correspondingVerticesRange;
        
        public Tile3dDescriptor Descriptor;
        
        public Tile3d(Mesh gridMesh)
        {
            correspondingVerticesRange = new IntRange(-1, -1);
            Descriptor = Tile3dDescriptor.Default;
            this.gridMesh = gridMesh;
        }
        public void SetVerticesRange(IntRange verticesRange)
        {
            correspondingVerticesRange = verticesRange;
        }
        
        public void ReplaceAttributes(Tile3d tile)
        {
            this.Descriptor = tile.Descriptor;
        }
        public void Paint(Color color)
        {
            if(gridMesh != null)
                gridMesh.Color32Vertices(correspondingVerticesRange, color);
        }
        
        public Tile3d Clone()
        {
            var clone = new Tile3d(gridMesh);
            clone.correspondingVerticesRange = correspondingVerticesRange;
            clone.Descriptor = Descriptor;
            
            return clone;
        }
        public void DebugTile()
        {
            Debug.Log($"Tile Debug: {Descriptor.Id}; [{correspondingVerticesRange.Start}, {correspondingVerticesRange.End}]");
        }
        
    }
}