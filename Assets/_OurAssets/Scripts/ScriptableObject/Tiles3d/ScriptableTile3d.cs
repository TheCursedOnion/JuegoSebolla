using NaughtyAttributes;
using UnityEngine;

namespace CursedOnion.Game.Systems.Grid.Scriptable
{
    [CreateAssetMenu(fileName = "Scriptable Tile3d", menuName = "Game/Tile/Scriptable Tile")]
    public class ScriptableTile3d : ScriptableObject
    {
        public Tile3dDescriptor TileDescriptor;
        public Tile3d ProduceTileForMesh(Mesh gridMesh)
        {
            var tile3d = new Tile3d(gridMesh)
            {
                Descriptor = TileDescriptor
            };
            return tile3d.Clone();
        }
    }
}
