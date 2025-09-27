using CursedOnion.Extensions;
using CursedOnion.Game.Systems.Grid.Scriptable;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CursedOnion.Game.Systems.Grid
{
    [System.Serializable]
    public class Grid3d
    {
        [SerializeField] private Vector3 origin;
            public Vector3 Origin => origin;
            
        [SerializeField] private Vector3Int size;
            public Vector3Int Size => size;

        [SerializeField] private ITile3d[] tiles;
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
            tiles = new ITile3d[size.x * size.y * size.z];
            if(layers == null || layers.Length == 0) return;
            
            foreach (var layer in layers)
            {
                foreach (Transform transform in layer.transform)
                {
                    Vector3 worldPositon = transform.position;
                    
                    var tileDefinition = transform.gameObject.GetComponent<Tile3dComponent>()?.tile;
                    var tile = tileDefinition != null ? tileDefinition.ProduceTile() : ITile3d.Default;
                    
                    SetTileAtWorldPosition(worldPositon, tile);
                }
            }
        }

        public ITile3d GetTileAtWorldPosition(Vector3 worldPosition)
        {
            var shiftedPosition = (worldPosition - origin);
            var vectorIndex = shiftedPosition.ConvertToVectorInt();

            int index = vectorIndex.x + vectorIndex.z * size.x + vectorIndex.y * size.x * size.z;
            return tiles[index] ??= ITile3d.Default;
        }
        public ITile3d GetTileAtGridPosition(Vector3 gridPosition)
        {
            var vectorIndex = gridPosition.ConvertToVectorInt();
            
            int index = vectorIndex.x + vectorIndex.z * size.x + vectorIndex.y * size.x * size.z;
            return tiles[index] ??= ITile3d.Default;
        }

        public void SetTileAtWorldPosition(Vector3 worldPosition, ITile3d tile)
        {
            var shiftedPosition = (worldPosition - origin);
            var vectorIndex = shiftedPosition.ConvertToVectorInt();

            tiles[vectorIndex.x + vectorIndex.z * size.x + vectorIndex.y * size.x * size.z] = tile;
        }
        public void SetTileAtGridPosition(Vector3 gridPosition, ITile3d tile)
        {
            var vectorIndex = gridPosition.ConvertToVectorInt();
            tiles[vectorIndex.x + vectorIndex.z * size.x + vectorIndex.y * size.x * size.z] = tile;
        }

        public void DebugGrid()
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                var tile = tiles[i] ??= ITile3d.Default;
                tiles[i].DebugTile();
            }
            Debug.Log(tiles.Length);
            Debug.Log(size);
        }
    }
}
