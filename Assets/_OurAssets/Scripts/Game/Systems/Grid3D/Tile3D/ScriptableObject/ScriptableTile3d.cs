using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CursedOnion.Game.Systems.Grid.Scriptable
{
    [CreateAssetMenu(fileName = "Scriptable Tile3d", menuName = "Game/Tile/Scriptable Tile")]
    public class ScriptableTile3d : ScriptableObject
    {
        public Tile3dDescriptor TileDescriptor;
        public Tile3d ProduceTile(Transform tileComponentTransform, TileAttributes tileAttributes)
        {
            var tile3d = new Tile3d(TileDescriptor, tileAttributes);
            
            var rotation = tileComponentTransform.rotation.eulerAngles;
            tile3d.RotateTile(rotation.y);
            
            return tile3d;
        }
    }
    
    [CreateAssetMenu(fileName = "Scriptable TileFlags", menuName = "Game/Tile/Scriptable TileFlags")]
    public class ScriptableTileAttributes : ScriptableObject
    {
        public TileAttributes TileAttributes;
    }
}
