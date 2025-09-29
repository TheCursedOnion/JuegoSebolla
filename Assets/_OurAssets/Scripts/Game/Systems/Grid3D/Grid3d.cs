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
        

        #region Constructors
        public Grid3d(Vector3 size, Vector3 origin, Mesh gridMesh, Tilemap[] layers)
        {
            this.size = size.ObtainVectorInt();
            
            origin.Floor();
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
        public bool TryWorldToGridPosition(Vector3 worldPosition, out Vector3 gridPosition)
        {
            origin.Floor();
            gridPosition = worldPosition - origin;
            gridPosition.Truncate();
            if (IsGridPositionInBounds(gridPosition))
            {
                return true;
            }

            gridPosition = Vector3.zero;
            return false;
        }
        public bool TryWorldPositionToIndex(Vector3 worldPosition, out int gridIndex)
        {
            gridIndex = -1;
            if(!TryWorldToGridPosition(worldPosition, out var gridPosition)) return false;
            
            Vector3Int vectorIndex = gridPosition.ObtainVectorInt();
            gridIndex = vectorIndex.x + vectorIndex.z * size.x + vectorIndex.y * size.x * size.z;
            return true;
        }

        public bool TryGridPositionToIndex(Vector3 gridPosition, out int gridIndex)
        {
            gridIndex = -1;
            if(!IsGridPositionInBounds(gridPosition)) return false;
                
            Vector3Int vectorIndex = gridPosition.ObtainVectorInt();
            gridIndex = vectorIndex.x + vectorIndex.z * size.x + vectorIndex.y * size.x * size.z;
            return true;
        }
        public bool IsGridPositionInBounds(Vector3 gridPosition)
        {
            return
                gridPosition.x >= 0 && gridPosition.x < size.x &&
                gridPosition.y >= 0 && gridPosition.y < size.y &&
                gridPosition.z >= 0 && gridPosition.z < size.z;
        }

        public Tile3d GetTileAtWorldPosition(Vector3 worldPosition)
        {
            if (TryWorldPositionToIndex(worldPosition, out int gridIndex))
            {
                return tiles[gridIndex] ??= Tile3d.Default;
            }
            return null;
        }
        public Tile3d GetTileAtGridPosition(Vector3 gridPosition)
        {
            if (TryGridPositionToIndex(gridPosition, out int gridIndex))
            {
                return tiles[gridIndex] ??= Tile3d.Default;
            }
            Debug.LogWarning("afu");
            return null;
        }

        public void SetTileAtWorldPosition(Vector3 worldPosition, Tile3d tile)
        {
            if (TryWorldPositionToIndex(worldPosition, out int gridIndex))
            {
                SetOrReplaceTile(gridIndex, tile);
            }
        }
        public void SetTileAtGridPosition(Vector3 gridPosition, Tile3d tile)
        {
            if (TryGridPositionToIndex(gridPosition, out int gridIndex))
            {
                SetOrReplaceTile(gridIndex, tile);
            }
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
