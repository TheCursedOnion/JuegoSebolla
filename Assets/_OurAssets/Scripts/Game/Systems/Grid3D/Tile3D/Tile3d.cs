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
                var defaultTile = new Tile3d();
                defaultTile.Descriptor = Tile3dDescriptor.Default;
                return defaultTile;
            }
        }

        public IntRange VertexRange;
        public Tile3dDescriptor Descriptor;

        public Tile3d()
        {
            VertexRange = new IntRange(-1, -1);
            Descriptor = Tile3dDescriptor.Default;
        }
        
        public Tile3d Clone()
        {
            var clone = new Tile3d();
            clone.VertexRange = VertexRange;
            clone.Descriptor = Descriptor;
            
            return clone;
        }

        public void Replace(Tile3d tile)
        {
            this.Descriptor = tile.Descriptor;
        }
        public void DebugTile()
        {
            Debug.Log($"Tile Debug: {Descriptor.Id}; [{VertexRange.Start}, {VertexRange.End}]");
        }
        
    }
}