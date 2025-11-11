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

        public List<Vector3> GetReachablePositions(Vector3 startWorldPos, int minRange, int maxRange)
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

                if (distance >= minRange && distance <= maxRange)
                    reachable.Add(current);

                if (distance >= maxRange)
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
                return null;

            Vector3Int start = new Vector3Int(
                Mathf.FloorToInt(startGrid.x),
                Mathf.FloorToInt(startGrid.y),
                Mathf.FloorToInt(startGrid.z)
            );

            List<Vector3> reachablePositions = new List<Vector3>();
            Queue<(Vector3Int pos, int cost)> frontier = new();
            HashSet<Vector3Int> visited = new();

            frontier.Enqueue((start, 0));
            visited.Add(start);

            Vector3Int[] directions =
            {
                new Vector3Int(1, 0, 0),
                new Vector3Int(-1, 0, 0),
                new Vector3Int(0, 0, 1),
                new Vector3Int(0, 0, -1)
            };

            while (frontier.Count > 0)
            {
                var (currentAirPos, cost) = frontier.Dequeue();
                if (cost > 0)
                    reachablePositions.Add(currentAirPos);

                if (cost >= movementRange)
                    continue;

                // Obtenemos la tile de suelo debajo de la unidad
                Vector3Int currentGroundPos = currentAirPos + Vector3Int.down;
                Tile3d groundTile = GetTileAtGridPosition(currentGroundPos);
                if (groundTile == null)
                    continue;

                var exitDirs = groundTile.GetExitDirections();
                var entryDirs = groundTile.GetEntryDirections();

                foreach (var dir in directions)
                {
                    //Tile de aire adyacente (posición donde estaría el personaje si avanza)
                    Vector3Int nextAirPos = currentAirPos + dir;
                    if (!IsGridPositionInBounds(nextAirPos))
                        continue;

                    Tile3d nextAirTile = GetTileAtGridPosition(nextAirPos);
                    if (nextAirTile.GetContainedEntity() != null)
                        continue;

                    if (visited.Contains(nextAirPos))
                        continue;

                    //Tile de suelo debajo del nuevo aire
                    Vector3Int nextGroundPos = nextAirPos + Vector3Int.down;
                    if (!IsGridPositionInBounds(nextGroundPos))
                        continue;

                    Tile3d nextGroundTile = GetTileAtGridPosition(nextGroundPos);
                    if (nextGroundTile == null)
                        continue;

                    var nextDesc = nextGroundTile.GetTileDescriptor();

                    // Si no hay suelo abajo no puedes pisar
                    if (nextDesc.IsAirBlock)
                        continue;

                    //Comprobamos direcciones
                    DirectionFlag moveDir = DirectionHelper.GetDirectionFlag(dir);
                    DirectionFlag opposite = DirectionHelper.GetDirectionFlag(-dir);

                    var groundExit = groundTile.GetExitDirections();
                    var nextEntry = nextGroundTile.GetEntryDirections();

                    if ((groundExit & moveDir) != 0 && (nextEntry & opposite) != 0)
                    {
                        //Es un movimiento válido añadimos el aire encima del nuevo suelo
                        frontier.Enqueue((nextAirPos, cost + nextDesc.Cost + 1));
                        visited.Add(nextAirPos);
                    }
                }
            }

            return reachablePositions;
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

                // Condición de "suelo sólido": no es aire y es bloque lleno
                if (!desc.IsAirBlock && desc.IsFullBlock)
                {
                    return check; 
                }

                if ((tile.GetExitDirections() & (DirectionFlag.ForwardDown | DirectionFlag.BackDown | DirectionFlag.LeftDown | DirectionFlag.RightDown)) != 0)
                {
                    return check;
                }
            }

            // Si no se encontró ningún suelo
            return Vector3Int.one * int.MinValue;
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
            var reachable = GetReachablePositionsMovement(startWorldPos, moveRange);

            foreach (var pos in reachable)
            {
                // Encontramos la "superficie sólida" de la tile (para pintar correctamente)
                Vector3Int groundPos = FindFirstSolidBelow(pos);
                if (groundPos == Vector3Int.one * int.MinValue)
                    continue;

                // Pintamos la tile en la posición de la superficie
                Tile3d tile = GetTileAtGridPosition(groundPos);
                if (tile != null)
                    PaintTile(tile, color);
            }
        }

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
