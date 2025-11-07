using CursedOnion.Game.Systems.Grid.Scriptable;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace CursedOnion.Game.Systems.Grid
{
    public class Tile3dComponent : MonoBehaviour
    {
        [FormerlySerializedAs("tile")] [Expandable] public ScriptableTile3d ScriptableTile;
        public TileAttributes SpecialAttributes;

        public Tile3d ProduceTile()
        {
            return ScriptableTile.ProduceTile(transform, SpecialAttributes);
        }
    }
    
    [System.Serializable]
    public class TileAttributes
    {
        public static TileAttributes Default
        {
            get
            {
                var defaultFlags = new TileAttributes
                {
                    CanUnitsSpawnHere = false
                };
                return defaultFlags;
            }
        }
        
        public bool CanUnitsSpawnHere;
    }
    
}