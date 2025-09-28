using System.Linq;
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
        public Grid3d(Vector3 size, Vector3 origin, Mesh gridMesh, Tilemap[] layers)
        {
            this.size = size.ConvertToVectorInt();
            this.origin = origin;
            
            InitializeTiles(gridMesh, layers);
        }
        void InitializeTiles(Mesh gridMesh, Tilemap[] layers)
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
                    Tile3d tile = tileDefinition != null ? tileDefinition.ProduceTileForMesh(gridMesh) : Tile3d.Default;
                    
                    int lastVertexIndex = vertexCount + tileMesh.vertices.Length - 1;
                    tile.SetVerticesRange(new IntRange(vertexCount, lastVertexIndex));
                    
                    SetTileAtWorldPosition(worldPositon, tile);
                    vertexCount += tileMesh.vertices.Length;
                }
            }
        }
        #endregion

        public bool WorldPositionInBounds(Vector3 worldPosition)
        {
            return IsIndexInBounds(GetIndexFromWorldPosition(worldPosition));
        }
        public bool AllWorldPositionsInBounds(Vector3[] worldPositions)
        {
            return worldPositions.All(pos => IsIndexInBounds(GetIndexFromWorldPosition(pos)));
        }
        public bool AllGridPositionsInBounds(Vector3[] gridPositions)
        {
            return gridPositions.All(pos => IsIndexInBounds(GetIndexFromGridPosition(pos)));
        }

        public bool IsGridPositionInBounds(Vector3 gridPosition)
        {
            return
                gridPosition.x >= 0 && gridPosition.x < size.x &&
                gridPosition.y >= 0 && gridPosition.y < size.y &&
                gridPosition.z >= 0 && gridPosition.z < size.z;
        }
        
        private bool IsIndexInBounds(int index)
        {
            return index >= 0 && index < tiles.Length;
        }

        private int GetIndexFromWorldPosition(Vector3 worldPosition)
        {
            Vector3 gridPosition = (worldPosition - origin);
            if(!IsGridPositionInBounds(gridPosition)) return -1;
            
            Vector3Int vectorIndex = gridPosition.ConvertToVectorInt();
            int index = vectorIndex.x + vectorIndex.z * size.x + vectorIndex.y * size.x * size.z;
            return index;
        }
        private int GetIndexFromGridPosition(Vector3 gridPosition)
        {
            Vector3Int vectorIndex = gridPosition.ConvertToVectorInt();
            if(!IsGridPositionInBounds(gridPosition)) return -1;
            
            int index = vectorIndex.x + vectorIndex.z * size.x + vectorIndex.y * size.x * size.z;
            return index;
        }

        public Tile3d GetTileAtWorldPosition(Vector3 worldPosition)
        {
            int index = GetIndexFromWorldPosition(worldPosition);
            return IsIndexInBounds(index) ? tiles[index] ??= Tile3d.Default : null;
        }
        public Tile3d GetTileAtGridPosition(Vector3 gridPosition)
        {
            int index = GetIndexFromGridPosition(gridPosition);
            return IsIndexInBounds(index) ? tiles[index] ??= Tile3d.Default : null;
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
        private void SetOrReplaceTile(int index, Tile3d tile)
        {
            if(!IsIndexInBounds(index)) return;

            if (tiles[index] != null)
            {
                tiles[index].ReplaceAttributes(tile);
            }
            else
            {
                tiles[index] = tile;
            }
        }
    }
}
