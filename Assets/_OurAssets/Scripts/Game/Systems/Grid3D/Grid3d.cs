using CursedOnion.Extensions;
using CursedOnion.Game;
using CursedOnion.Game.Systems.Files;
using CursedOnion.Game.Systems.Grid.Scriptable;
using CursedOnion.Helpers;
using CursedOnion.Tools;
using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using CursedOnion.Game.Modes.General;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CursedOnion.Game.Systems.Grid
{
    [System.Serializable]
    public class Grid3d
    {
        [SerializeField] private Mesh mesh;
        public Mesh Mesh => mesh;
        
        GridHighlighter highlighter;

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

        public void PrepareGrid(GridHighlighter highlighter)
        {
            this.highlighter = highlighter;
            this.highlighter.Initialize(this);
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


        public void InsertReachablePositions(List<Vector3> reachablePositions, Vector3 startWorldPos, int minRange, int maxRange)
        {
            ComputeReachablePositionsInternal(startWorldPos, minRange, maxRange, reachablePositions);
        }
        public List<Vector3> GetReachablePositions(Vector3 startWorldPos, int minRange, int maxRange)
        {
            var list = new List<Vector3>();
            ComputeReachablePositionsInternal(startWorldPos, minRange, maxRange, list);
            return list;
        }
        static readonly Vector3Int[] directions =
        {
            new(1, 0, 0),
            new(-1, 0, 0),
            new(0, 1, 0),
            new(0, -1, 0),
            new(0, 0, 1),
            new(0, 0, -1)
        };
        private void ComputeReachablePositionsInternal(Vector3 startWorldPos, int minRange, int maxRange, List<Vector3> output)
        {
            output.Clear();

            if (!TryWorldToGridPosition(startWorldPos, out Vector3 startGridPos))
                return;

            Vector3Int start = new Vector3Int(
                Mathf.FloorToInt(startGridPos.x),
                Mathf.FloorToInt(startGridPos.y),
                Mathf.FloorToInt(startGridPos.z)
            );

            var frontier = new Queue<(Vector3Int pos, int distance)>();
            var visited = new HashSet<Vector3Int>();

            frontier.Enqueue((start, 0));
            visited.Add(start);
            
            while (frontier.Count > 0)
            {
                var (current, distance) = frontier.Dequeue();

                if (distance >= minRange && distance <= maxRange)
                    output.Add(current);

                if (distance >= maxRange)
                    continue;

                foreach (var dir in directions)
                {
                    Vector3Int next = current + dir;

                    if (!IsGridPositionInBounds((Vector3)next)) continue;

                    if (!visited.Add(next)) continue;

                    frontier.Enqueue((next, distance + 1));
                }
            }
        }
        

        private Vector3Int FindFirstSolidBelow(Vector3 pos)
        {
            Vector3Int gridPos = new Vector3Int(
                Mathf.FloorToInt(pos.x),
                Mathf.FloorToInt(pos.y),
                Mathf.FloorToInt(pos.z)
            );

            for (int y = gridPos.y; y >= 0; y--)
            {
                Vector3Int check = new Vector3Int(gridPos.x, y, gridPos.z);
                Tile3d tile = GetTileAtGridPosition(check);
                if (tile == null)
                    continue;

                var desc = tile.GetTileDescriptor();

                // Condici�n de "suelo s�lido": no es aire y es bloque lleno
                if (!desc.IsAirBlock && desc.IsFullBlock)
                {
                    return check; 
                }

                if ((tile.GetExitDirections() & (DirectionFlag.ForwardDown | DirectionFlag.BackDown | DirectionFlag.LeftDown | DirectionFlag.RightDown)) != 0)
                {
                    return check;
                }
            }

            // Si no se encontr� ning�n suelo
            return Vector3Int.one * int.MinValue;
        }

        #endregion

        #region Painting
        
        List<HighlightPlane> highlightedPlanes = new();
        public void PaintTileAtWorldPosition(Vector3 worldPosition, Color color)
        {
            if(TryWorldToGridPosition(worldPosition, out Vector3 gridPosition)) 
                highlightedPlanes.Add(highlighter.PlaceHighlightPlaneAt(worldPosition, color));
        }
        public void PaintTileAtGridPosition(Vector3 gridPosition, Color color)
        {
            if (TryGridToWorldPosition(gridPosition, out Vector3 worldPosition))
                highlightedPlanes.Add(highlighter.PlaceHighlightPlaneAt(worldPosition.CenterOnTile(), color));
        }

        public void PaintTilesAtGridPositions(List<Vector3> positions, Color color)
        {
            Debug.Log(positions.Count);
            for (int i = 0; i < positions.Count; i++)
            {
                if(i==0) ResetPaint();
                PaintTileAtGridPosition(positions[i], color);
            }
        }
        
        public void ResetPaint()
        {
            Debug.Log("ResetPaint");
            if(highlightedPlanes.Count == 0) return;
            
            foreach (var plane in highlightedPlanes)
                highlighter.RetrieveHighlightPlane(plane);
            
            highlightedPlanes.Clear();
        }

        /*public void HighlightMovementRange(Vector3 startWorldPos, int moveRange, Color color)
        {
            var reachable = GetReachablePositionsMovement(startWorldPos, moveRange);

            foreach (var pos in reachable)
            {
                Vector3Int groundPos = FindFirstSolidBelow(pos);
                if (groundPos == Vector3Int.one * int.MinValue)
                    continue;
                
                PaintTileAtGridPosition(groundPos, color);
            }
        }*/

        public void HighlightActionRange(Vector3 startWorldPos, int minRange, int maxRange, Color color)
        {
            var reachable = GetReachablePositions(startWorldPos, minRange, maxRange);

            foreach (var pos in reachable)
            {
                Vector3 newPos = new Vector3(
                    pos.x,
                    pos.y - 1,
                    pos.z
                );
                PaintTileAtGridPosition(newPos, color);
            }
        }

        public void HighlightArcherAbilityRange(Vector3 startWorldPos, int minRange, int maxRange, Color color)
        {
            if (!TryWorldToGridPosition(startWorldPos, out Vector3 startGrid))
                return;

            Vector3Int start = new Vector3Int(
                Mathf.FloorToInt(startGrid.x),
                Mathf.FloorToInt(startGrid.y),
                Mathf.FloorToInt(startGrid.z)
            );

            Vector3Int[] directions = new Vector3Int[]
            {
                new Vector3Int(1, 0, 0),  
                new Vector3Int(-1, 0, 0), 
                new Vector3Int(0, 0, 1),  
                new Vector3Int(0, 0, -1)  
            };

            foreach (var dir in directions)
            {
                for (int dist = minRange; dist <= maxRange; dist++)
                {
                    Vector3Int pos = start + dir * dist;

                    if (!IsGridPositionInBounds((Vector3)pos))
                        break;

                    Vector3 newPos = new Vector3(
                        pos.x,
                        pos.y - 1,
                        pos.z
                    );

                    PaintTileAtGridPosition(newPos, color);

                    Tile3d tile = GetTileAtGridPosition(pos);
                    if (tile == null || tile.GetContainedEntity() != null)
                        break; 
                }
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
