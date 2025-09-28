using CursedOnion.Extensions;
using CursedOnion.Game.Systems.Files;
using CursedOnion.Game.Systems.Grid.Scriptable;
using CursedOnion.Helpers;
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

        [SerializeField] private Tile3d[] tiles;
        

        #region Constructos
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

            int vertexCount = 0;
            foreach (var layer in layers)
            {
                foreach (Transform transform in layer.transform)
                {
                    Vector3 worldPositon = transform.position;
                    Mesh tileMesh = transform.GetComponent<MeshFilter>().sharedMesh;
                    
                    ScriptableTile3d tileDefinition = transform.gameObject.GetComponent<Tile3dComponent>()?.tile;
                    Tile3d tile = tileDefinition != null ? tileDefinition.ProduceTile() : Tile3d.Default;
                    
                    int lastVertexIndex = vertexCount + tileMesh.vertices.Length - 1;
                    tile.VertexRange = new IntRange(vertexCount, lastVertexIndex);
                    
                    SetTileAtWorldPosition(worldPositon, tile);
                    vertexCount += tileMesh.vertices.Length;
                }
            }
        }
        #endregion

        private int GetIndexFromWorldPosition(Vector3 worldPosition)
        {
            Vector3 shiftedPosition = (worldPosition - origin);
            Vector3Int vectorIndex = shiftedPosition.ConvertToVectorInt();
            int index = vectorIndex.x + vectorIndex.z * size.x + vectorIndex.y * size.x * size.z;
            return index;
        }
        private int GetIndexFromGridPosition(Vector3 gridPosition)
        {
            Vector3Int vectorIndex = gridPosition.ConvertToVectorInt();
            int index = vectorIndex.x + vectorIndex.z * size.x + vectorIndex.y * size.x * size.z;
            return index;
        }

        public Tile3d GetTileAtWorldPosition(Vector3 worldPosition)
        {
            int index = GetIndexFromWorldPosition(worldPosition);
            return IsInBounds(index) ? tiles[index] ??= Tile3d.Default : null;
        }
        public Tile3d GetTileAtGridPosition(Vector3 gridPosition)
        {
            int index = GetIndexFromGridPosition(gridPosition);
            return IsInBounds(index) ? tiles[index] ??= Tile3d.Default : null;
        }

        public void SetTileAtWorldPosition(Vector3 worldPosition, Tile3d tile)
        {
            int index = GetIndexFromWorldPosition(worldPosition);
            SetOrReplaceTile(index, tile);
        }
        public void SetTileAtGridPosition(Vector3 gridPosition, Tile3d tile)
        {
            int index = GetIndexFromGridPosition(gridPosition);
            SetOrReplaceTile(index, tile);
        }

        public void DebugGrid()
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i] ??= Tile3d.Default;
                tiles[i].DebugTile();
            }
            Debug.Log(tiles.Length);
            Debug.Log(size);
        }

        private bool IsInBounds(int index)
        {
            return index >= 0 && index < tiles.Length;
        }

        private void SetOrReplaceTile(int index, Tile3d tile)
        {
            if(!IsInBounds(index)) return;

            if (tiles[index] != null)
            {
                tiles[index].Replace(tile);
            }
            else
            {
                tiles[index] = tile;
            }
        }
    }
}
