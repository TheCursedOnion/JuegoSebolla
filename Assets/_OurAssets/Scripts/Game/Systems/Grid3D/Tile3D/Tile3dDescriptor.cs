using CursedOnion.Game;
using CursedOnion.Game.Systems.Grid.Scriptable;
using CursedOnion.Helpers;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace CursedOnion.Game.Systems.Grid
{
    [System.Serializable]
    public class Tile3dDescriptor
    {
        public static Tile3dDescriptor Default
        {
            get
            {
                var defaultDescriptor = new Tile3dDescriptor
                {
                    Id = 0,
                    Cost = 0,
                    IsFullBlock = true,
                    IsFluidBlock = false,
                    IsAirBlock = true,
                };
                return defaultDescriptor;
            }
        }
        public uint Id;
        public int Cost;
        
        public bool IsFullBlock = true;
        public bool IsFluidBlock = false;
        public bool IsAirBlock = false;
        
        [FormerlySerializedAs("allowedExitDirections")] public DirectionFlag AllowedExitDirections;
        [FormerlySerializedAs("allowedEntryDirections")] public DirectionFlag AllowedEntryDirections;
    }
}