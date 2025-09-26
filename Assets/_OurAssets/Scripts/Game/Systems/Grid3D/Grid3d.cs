using CursedOnion.Extensions;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CursedOnion.Game.Grid
{
    public class Grid3d
    {
        private Vector3 origin;
            public Vector3 Origin => origin;
            
        private Vector3Int size;
            public Vector3Int Size => size;

        private Tile3d[] tiles;

        public Grid3d(Vector3 size, Vector3 origin)
        {
            this.size = size.ConvertToVectorInt();
            this.origin = origin;
            
            InitializeTiles(null);
        }

        public Grid3d(Vector3 size, Vector3 origin, Tilemap[] layers)
        {
            this.size = size.ConvertToVectorInt();
            this.origin = origin;
            
            InitializeTiles(layers);
        }

        void InitializeTiles(Tilemap[] layers)
        {
            tiles = new Tile3d[size.x * size.y * size.z];
            if(layers == null || layers.Length == 0) return;
            
            foreach (var layer in layers)
            {
                foreach (Transform transform in layer.transform)
                {
                    Vector3 worldPositon = transform.position;
                    Tile3d tileInTransform = transform.gameObject.GetComponent<Tile3dComponent>()?.CreateTileFromComponent();
                    SetTileAtWorldPosition(worldPositon, tileInTransform);
                    Debug.Log(worldPositon);
                }
            }
        }

        public Tile3d GetTileAtWorldPosition(Vector3 worldPosition)
        {
            var shiftedPosition = (worldPosition - origin);
            var vectorIndex = shiftedPosition.ConvertToVectorInt();

            int index = vectorIndex.x + vectorIndex.z * size.x + vectorIndex.y * size.x * size.z;
            return tiles[index] ??= new AirTile();
        }
        public Tile3d GetTileAtGridPosition(Vector3 gridPosition)
        {
            var vectorIndex = gridPosition.ConvertToVectorInt();
            
            int index = vectorIndex.x + vectorIndex.z * size.x + vectorIndex.y * size.x * size.z;
            return tiles[index] ??= new AirTile();
        }

        public void SetTileAtWorldPosition(Vector3 worldPosition, Tile3d tile)
        {
            var shiftedPosition = (worldPosition - origin);
            var vectorIndex = shiftedPosition.ConvertToVectorInt();

            tiles[vectorIndex.x + vectorIndex.z * size.x + vectorIndex.y * size.x * size.z] = tile;
        }
        public void SetTileAtGridPosition(Vector3 gridPosition, Tile3d tile)
        {
            var vectorIndex = gridPosition.ConvertToVectorInt();
            tiles[vectorIndex.x + vectorIndex.z * size.x + vectorIndex.y * size.x * size.z] = tile;
        }
    }
}
