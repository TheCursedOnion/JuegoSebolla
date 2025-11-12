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
                    Cost = 0,
                    IsFullBlock = false,
                    IsFluidBlock = false,
                    IsAirBlock = true,
                    AllowedEntryDirections = DirectionFlag.CentralXZ,
                    AllowedExitDirections = DirectionFlag.CentralXZ,
                };
                return defaultDescriptor;
            }
        }
        
        public int Cost;
        
        public bool IsFullBlock = true;
        public bool IsFluidBlock = false;
        public bool IsAirBlock = false;
        
        [FormerlySerializedAs("allowedExitDirections")] public DirectionFlag AllowedExitDirections;
        [FormerlySerializedAs("allowedEntryDirections")] public DirectionFlag AllowedEntryDirections;
    }
}