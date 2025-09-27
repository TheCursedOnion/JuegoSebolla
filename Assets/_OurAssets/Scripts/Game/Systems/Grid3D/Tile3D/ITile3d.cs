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
                var defaultTile = new AirTile();
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
    public class AirTile : ITile3d
    {
        public Tile3dDescriptor Descriptor { get; set; }
        public ITile3d Clone()
        {
            var clone = new AirTile();
            clone.Descriptor = Descriptor;
            
            return clone;
        }
    }
    
    [System.Serializable]
    public class GrassTile : ITile3d
    {
        public Tile3dDescriptor Descriptor { get; set; }
        public ITile3d Clone()
        {
            var clone = new GrassTile();
            clone.Descriptor = Descriptor;
            
            return clone;
        }
    }
}