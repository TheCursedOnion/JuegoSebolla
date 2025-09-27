using NaughtyAttributes;
using UnityEngine;

namespace CursedOnion.Game.Systems.Grid.Scriptable
{
    [CreateAssetMenu(fileName = "Scriptable Tile3d", menuName = "Game/Tile/Scriptable Tile")]
    public class ScriptableTile3d : ScriptableObject
    {
        [SerializeReference, SubclassSelector] ITile3d tile3d;
        
        [ShowIf("TryShow")][Header("Tile Descriptor")]
        public Tile3dDescriptor descriptor;
            public bool TryShow() { return tile3d != null;}
        public ITile3d ProduceTile()
        {
            tile3d.Descriptor = descriptor;
            return tile3d.Clone();
        }
    }
}
