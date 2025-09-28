using NaughtyAttributes;
using UnityEngine;

namespace CursedOnion.Game.Systems.Grid.Scriptable
{
    [CreateAssetMenu(fileName = "Scriptable Tile3d", menuName = "Game/Tile/Scriptable Tile")]
    public class ScriptableTile3d : ScriptableObject
    {
        public Tile3dDescriptor descriptor;
        public Tile3d ProduceTile()
        {
            var tile3d = new Tile3d
            {
                Descriptor = descriptor
            };
            return tile3d.Clone();
        }
    }
}
