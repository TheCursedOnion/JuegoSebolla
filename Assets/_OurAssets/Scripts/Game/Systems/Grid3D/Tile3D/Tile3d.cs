using UnityEngine;
using UnityEngine.Tilemaps;

namespace CursedOnion.Game.Grid
{
    public class Tile3dComponent : MonoBehaviour
    {
        public Tile3d CreateTileFromComponent()
        {
            return new AirTile();
        }
    }
    public class Tile3d
    {
        
    }

    public class AirTile : Tile3d
    {
        
    }
}