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
                    Tile3d tile = tileDefinition != null ? tileDefinition.ProduceTile() : Tile3d.Default;
                    
                    int lastVertexIndex = vertexCount + tileMesh.vertices.Length - 1;
                    tile.SetTileMeshProperties(gridMesh, new IntRange(vertexCount, lastVertexIndex));
                    
                    SetTileAtWorldPosition(worldPositon, tile);
                    vertexCount += tileMesh.vertices.Length;
                }
            }
        }
        #endregion
        public bool IsGridPositionInBounds(Vector3 gridPosition)
        {
            return
                gridPosition.x >= 0 && gridPosition.x < size.x &&
                gridPosition.y >= 0 && gridPosition.y < size.y &&
                gridPosition.z >= 0 && gridPosition.z < size.z;
        }
        public Tile3d GetTileAtWorldPosition(Vector3 worldPosition)
        {
            
            if (VectorConversions.TryWorldPositionToGridIndex(worldPosition, this, out int gridIndex))
            {
                return tiles[gridIndex] ??= Tile3d.Default;
            }
            return null;
        }
        public Tile3d GetTileAtGridPosition(Vector3 gridPosition)
        {
            if (VectorConversions.TryGridPositionToIndex(gridPosition, this, out int gridIndex))
            {
                return tiles[gridIndex] ??= Tile3d.Default;
            }
            return null;
        }

        public void SetTileAtWorldPosition(Vector3 worldPosition, Tile3d tile)
        {
            if (VectorConversions.TryWorldPositionToGridIndex(worldPosition, this, out int gridIndex))
            {
                SetOrReplaceTile(gridIndex, tile);
            }
        }
        public void SetTileAtGridPosition(Vector3 gridPosition, Tile3d tile)
        {
            if (VectorConversions.TryGridPositionToIndex(gridPosition, this, out int gridIndex))
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
