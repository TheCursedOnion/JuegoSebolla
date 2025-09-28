using CursedOnion.Game.Systems.Grid.Scriptable;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CursedOnion.Game.Systems.Grid
{
    public interface ITile3d
    {
        public static ITile3d Default
        {
            get
            {
                var defaultTile = new Tile3d();
                defaultTile.Descriptor = Tile3dDescriptor.Default;
                return defaultTile;
            }
        }
        
        public Tile3dDescriptor Descriptor { get; set; }
        public ITile3d Clone();

        public void DebugTile()
        {
            Debug.Log($"Tile Debug: {Descriptor.Id}");
        }
    }
    
    [System.Serializable]
    public class Tile3d : ITile3d
    {
        public Tile3dDescriptor Descriptor { get; set; }
        public ITile3d Clone()
        {
            var clone = new Tile3d();
            clone.Descriptor = Descriptor;
            
            return clone;
        }
    }
}