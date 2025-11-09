using CursedOnion.Extensions;
using CursedOnion.Game;
using CursedOnion.Game.Systems.Files;
using CursedOnion.Game.Systems.Grid.Scriptable;
using CursedOnion.Helpers;
using CursedOnion.Tools;
using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CursedOnion.Game.Systems.Grid
{
    [System.Serializable]
    public class Grid3d
    {
        [SerializeField] private Mesh mesh;
        public Mesh Mesh => mesh;

        [HorizontalLine]
        [SerializeField] private Vector3 startingOffset;
        public Vector3 StartingOffset { get => startingOffset; set => startingOffset = value; }

        [SerializeField] private Vector3 origin;
        public Vector3 Origin => origin;

        [SerializeField] private Vector3Int size;
        public Vector3Int Size => size;

        [SerializeField] private Tile3d[] tiles;

        #region Constructor
        public Grid3d(Vector3 size, Vector3 origin, Tilemap[] layers)
        {
            this.size = size.CastToVectorInt();

            origin.Floor();
            this.origin = origin;

            InitializeTiles();
            PlaceLayers(layers);
        }
        void InitializeTiles()
        {
            tiles = new Tile3d[size.x * size.y * size.z];

            for (int index = 0; index < tiles.Length; index++)
            {
                Tile3d tile = Tile3d.Default;
                SetTileAtIndex(index, tile);
            }
        }
        void PlaceLayers(Tilemap[] layers)
        {
            if (layers == null || layers.Length == 0) return;

            foreach (var layer in layers)
            {
                foreach (Transform transform in layer.transform)
                {
                    Vector3 worldPositon = transform.position;

                    Tile3dComponent tileComponent = transform.gameObject.GetComponent<Tile3dComponent>();
                    Tile3d tile = tileComponent != null ? tileComponent.ProduceTile() : Tile3d.Default;

                    SetTileAtWorldPosition(worldPositon, tile);
                }
            }
        }

        public void SetMeshForGrid(Mesh gridMesh)
        {
            mesh = gridMesh;
        }
        #endregion

        #region Conversions
        public bool TryWorldToGridPosition(Vector3 worldPosition, out Vector3 gridPosition)
        {
            return VectorConversions.TryWorldToGridPosition(worldPosition + startingOffset, this, out gridPosition);
        }
        public bool TryGridToWorldPosition(Vector3 gridPosition, out Vector3 worldPosition)
        {
            bool result = VectorConversions.TryGridToWorldPosition(gridPosition, this, out worldPosition);
            worldPosition -= startingOffset;
            return result;
        }
        public bool TryWorldPositionToGridIndex(Vector3 worldPosition, out int gridIndex)
        {
            return VectorConversions.TryWorldPositionToGridIndex(worldPosition + startingOffset, this, out gridIndex);
        }
        public bool TryGridPositionToIndex(Vector3 gridPosition, out int gridIndex)
        {
            return VectorConversions.TryGridPositionToIndex(gridPosition, this, out gridIndex);
        }
        public int GridPositionToIndex(Vector3 gridPosition, Grid3d grid)
        {
            Vector3Int gridSize = grid.Size;
            Vector3Int vectorIndex = gridPosition.CastToVectorInt();
            return vectorIndex.x + vectorIndex.z * gridSize.x + vectorIndex.y * gridSize.x * gridSize.z;
        }
        public bool IsGridPositionInBounds(Vector3 gridPosition)
        {
            return
                gridPosition.x >= 0 && gridPosition.x < size.x &&
                gridPosition.y >= 0 && gridPosition.y < size.y &&
                gridPosition.z >= 0 && gridPosition.z < size.z;
        }

        public bool IsIndexInBounds(int index)
        {
            return index >= 0 && index < tiles.Length;
        }
        #endregion

        #region Tile Getters & Setters
        public Tile3d GetTileAtWorldPosition(Vector3 worldPosition)
        {
            if (TryWorldPositionToGridIndex(worldPosition, out int gridIndex))
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
            return null;
        }
        public void SetTileAtWorldPosition(Vector3 worldPosition, Tile3d tile)
        {
            if (TryWorldPositionToGridIndex(worldPosition, out int gridIndex))
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
        public void SetTileAtIndex(int index, Tile3d tile)
        {
            if (IsIndexInBounds(index))
            {
                SetOrReplaceTile(index, tile);
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

        public List<Vector3> GetReachablePositions(Vector3 startWorldPos, int moveRange)
        {
            var reachable = new List<Vector3>();

            if (!TryWorldToGridPosition(startWorldPos, out Vector3 startGridPos))
                return reachable;

            Vector3Int start = new Vector3Int(
                Mathf.FloorToInt(startGridPos.x),
                Mathf.FloorToInt(startGridPos.y),
                Mathf.FloorToInt(startGridPos.z)
            );

            Queue<(Vector3Int pos, int distance)> frontier = new();
            HashSet<Vector3Int> visited = new();

            frontier.Enqueue((start, 0));
            visited.Add(start);

            Vector3Int[] directions = new Vector3Int[]
            {
                new Vector3Int(1, 0, 0),
                new Vector3Int(-1, 0, 0),
                new Vector3Int(0, 1, 0),
                new Vector3Int(0, -1, 0),
                new Vector3Int(0, 0, 1),
                new Vector3Int(0, 0, -1)
            };

            while (frontier.Count > 0)
            {
                var (current, distance) = frontier.Dequeue();

                if (distance > 0 && distance <= moveRange)
                    reachable.Add(current);

                if (distance >= moveRange)
                    continue;

                foreach (var dir in directions)
                {
                    Vector3Int next = current + dir;

                    if (!IsGridPositionInBounds((Vector3)next)) continue;
                    if (visited.Contains(next)) continue;

                    visited.Add(next);
                    frontier.Enqueue((next, distance + 1));
                }
            }
            return reachable;
        }

        public List<Vector3> GetReachablePositionsMovement(Vector3 startWorldPos, int movementRange)
        {
            if (!TryWorldToGridPosition(startWorldPos, out Vector3 startGrid))
            { return null; }

            List<Vector3> reachablePositions = new List<Vector3>();
            Queue<(Vector3 pos, int cost)> frontier = new Queue<(Vector3, int)>();
            HashSet<Vector3> visited = new HashSet<Vector3>();

            frontier.Enqueue((startGrid, 0));
            visited.Add(startGrid);

            while (frontier.Count > 0)
            {
                var (currentPos, currentCost) = frontier.Dequeue();
                reachablePositions.Add(currentPos);

                foreach (Vector3 neighbour in GetNeighboursMovement(currentPos, this))
                {
                    if (visited.Contains(neighbour))
                        continue;

                    Tile3d tile = GetTileAtGridPosition(neighbour);
                    if (tile == null || tile.GetContainedEntity() != null)
                        continue;

                    int newCost = currentCost + 1;
                    if (newCost <= movementRange)
                    {
                        frontier.Enqueue((neighbour, newCost));
                        visited.Add(neighbour);
                    }
                }
            }

            return reachablePositions;
        }
        private List<Vector3> GetNeighboursMovement(Vector3 gridPos, Grid3d levelGrid)
        {
            List<Vector3> neighbours = new List<Vector3>();
            List<Vector3> directions = new List<Vector3>
            {
                new Vector3( 1, 0,  0),
                new Vector3(-1, 0,  0),
                new Vector3( 0, 0,  1),
                new Vector3( 0, 0, -1),
            };

            foreach (var dir in directions)
            {
                Vector3 neighbour = gridPos + dir;

                if (levelGrid.GetTileAtGridPosition(neighbour) != null)
                    neighbours.Add(neighbour);
            }

            return neighbours;
        }
        #endregion

        #region Painting

        public Mesh PaintTile(Tile3d tile, Color color)
        {
            mesh.Color32Vertices(tile.CorrespondingVerticesInMesh, color);
            return mesh;
        }
        public Mesh PaintTileAtGridPosition(Vector3 gridPosition, Color color)
        {
            if (TryGridPositionToIndex(gridPosition, out int gridIndex))
            {
                var vertexRange = tiles[gridIndex].CorrespondingVerticesInMesh;
                mesh.Color32Vertices(vertexRange, color);
            }

            return mesh;
        }

        public Mesh PaintAllTiles(Color color)
        {
            for (int index = 0; index < tiles.Length; index++)
            {
                var vertexRange = tiles[index].CorrespondingVerticesInMesh;
                mesh.Color32Vertices(vertexRange, color);
            }
            return mesh;
        }
        public Mesh ResetPaint()
        {
            return PaintAllTiles(Color.white);
        }

        public void HighlightMovementRange(Vector3 startWorldPos, int moveRange, Color color)
        {
            var reachable = GetReachablePositionsMovement(startWorldPos, moveRange + 1);

            foreach (var pos in reachable)
            {
                PaintTileAtGridPosition(pos, color);
            }
        }

        public void HighlightActionRange(Vector3 startWorldPos, int moveRange, Color color)
        {
            var reachable = GetReachablePositions(startWorldPos, moveRange + 1);

            foreach (var pos in reachable)
            {
                PaintTileAtGridPosition(pos, color);
            }
        }

        #endregion
        public void DebugGrid()
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i] ??= Tile3d.Default;
                tiles[i].DebugTile();
            }
        }
    }
}
