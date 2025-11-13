using CursedOnion.Game.Systems.Grid;
using CursedOnion.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace CursedOnion.Game.Entity
{
    public static class AStarPathFinder
    {
        private static Vector3[] CentralXZ =
        {
            new(1, 0, 0),
            new(-1, 0, 0),
            new(0, 0, 1),
            new(0, 0, -1)
        };
        private static Vector3[] AllXZ =
        {
            new(1, 0, 0),
            new(-1, 0, 0),
            new(0, 0, 1),
            new(0, 0, -1),
            new(0.5f, 0, 0.5f),
            new(-0.5f, 0, 0.5f),
            new(0.5f, 0, -0.5f),
            new(-0.5f, 0, -0.5f),
        };
        public static void InsertMeleeAttackPositions(List<Vector3> reachablePositions, Grid3d grid, Vector3 startGridPos)
        {
            Tile3d tile = grid.GetTileAtGridPosition(startGridPos);
            var possibleDirections = tile.GetExitDirectionVector();
            
            reachablePositions.Clear();
            foreach (var direction in possibleDirections)
            {
                var newPos = startGridPos + direction;

                if (grid.TryGetTileAtGridPosition(newPos, out var nextTile) && !nextTile.IsFullTile()) 
                    reachablePositions.Add(newPos);
            }
        }
        public static void InsertRangeAttackPositions(List<Vector3> reachablePositions, Grid3d grid, Vector3 startGridPos, int distance, bool includeDiagonals)
        {
            reachablePositions.Clear();
            
            Vector3 up = Vector3.up;
            Vector3 down = Vector3.down;

            var usedDirections = includeDiagonals ? AllXZ : CentralXZ;
            foreach (var dir in usedDirections)
            {
                Vector3 targetPos = startGridPos + new Vector3(dir.x * distance, dir.y, dir.z * distance);

                if (!grid.TryGetTileAtGridPosition(targetPos, out Tile3d targetTile))
                    continue;

                bool isTargetFull = targetTile.IsFullTile();
                bool isTargetEmpty = targetTile.IsEmptyTile();
                
                if (isTargetFull && grid.TryGetTileAtGridPosition(targetPos + up, out Tile3d above) && above.IsEmptyTile())
                {
                    reachablePositions.Add(targetPos + up);
                    continue;
                }

                if (!isTargetFull && !isTargetEmpty)
                {
                    reachablePositions.Add(targetPos);
                    continue;
                }
                
                if (isTargetEmpty)
                {
                    if (grid.TryGetTileAtGridPosition(targetPos + down, out Tile3d below))
                    {
                        if (!below.IsEmptyTile())
                        {
                            if(below.IsFullTile())
                                reachablePositions.Add(targetPos);
                            else
                                reachablePositions.Add(targetPos + down);
                        }
                        else if (grid.TryGetTileAtGridPosition(targetPos + down * 2, out Tile3d lastBelow)
                            && !lastBelow.IsEmptyTile())
                        {
                            reachablePositions.Add(targetPos + down);
                        }
                    }
                }
            }
        }
        
        public static async Task InsertReachablePositionsAsyncBFS(
            List<Vector3> positions,
            Grid3d levelGrid,
            Vector3 startWorldPos,
            int movementRange,
            int yieldFrequency = 100)
        {
            if (!levelGrid.TryWorldToGridPosition(startWorldPos, out Vector3 startGrid))
                return;

            var frontier = new Queue<(Vector3 pos, int cost)>();
            var visited = new HashSet<Vector3>();

            positions.Clear();

            void TryAdd(Vector3 pos, int cost)
            {
                if (!visited.Contains(pos))
                {
                    frontier.Enqueue((pos, cost));
                    visited.Add(pos);
                    positions.Add(pos);
                }
            }

            frontier.Enqueue((startGrid, 0));
            visited.Add(startGrid);

            int iterations = 0;

            while (frontier.Count > 0)
            {
                var (currentAirPos, currentCost) = frontier.Dequeue();

                if (!levelGrid.IsGridPositionInBounds(currentAirPos))
                    continue;

                Tile3d currentAirTile = levelGrid.GetTileAtGridPosition(currentAirPos);
                if (currentAirTile == null)
                    continue;

                var currentDesc = currentAirTile.GetTileDescriptor();
                bool isOnStair = !currentDesc.IsAirBlock && !currentDesc.IsFullBlock;

                // --- Escaleras: movimiento dentro de la escalera ---
                if (isOnStair)
                {
                    foreach (var stairDir in currentAirTile.GetExitDirectionVector())
                    {
                        Vector3 stairTopPos = currentAirPos + stairDir;
                        if (!levelGrid.IsGridPositionInBounds(stairTopPos)) continue;

                        Tile3d stairTopTile = levelGrid.GetTileAtGridPosition(stairTopPos);
                        if (stairTopTile != null && !stairTopTile.IsBlocked())
                            TryAdd(stairTopPos, currentCost + currentDesc.Cost);
                    }
                }

                // --- Movimientos horizontales y diagonales ---
                foreach (var possibleDirection in currentAirTile.GetExitDirectionVector())
                {
                    Vector3 nextAirPos = currentAirPos + possibleDirection;
                    if (!levelGrid.IsGridPositionInBounds(nextAirPos)) continue;

                    Tile3d nextAirTile = levelGrid.GetTileAtGridPosition(nextAirPos);
                    if (nextAirTile == null || nextAirTile.IsBlocked())
                        continue;

                    var nextAirDescriptor = nextAirTile.GetTileDescriptor();

                    // --- Subir escalera ---
                    if (!nextAirDescriptor.IsAirBlock && !nextAirDescriptor.IsFullBlock)
                    {
                        Vector3 stairPos = nextAirPos;
                        Tile3d stairTile = levelGrid.GetTileAtGridPosition(stairPos);
                        if (stairTile == null || stairTile.IsBlocked()) continue;

                        if (nextAirTile.CanBeAccessedFrom(possibleDirection))
                        {
                            TryAdd(stairPos, currentCost + nextAirDescriptor.Cost);

                            Vector3 stairTopPos = stairPos + possibleDirection + Vector3.up;
                            if (levelGrid.IsGridPositionInBounds(stairTopPos))
                            {
                                Tile3d stairTopTile = levelGrid.GetTileAtGridPosition(stairTopPos);
                                if (stairTopTile != null && !stairTopTile.IsBlocked())
                                    TryAdd(stairTopPos, currentCost + nextAirDescriptor.Cost);
                            }
                        }
                    }

                    // --- Bajar escalera ---
                    Vector3 stairDownPos = currentAirPos + possibleDirection + Vector3.down;
                    if (levelGrid.IsGridPositionInBounds(stairDownPos))
                    {
                        Tile3d stairDownTile = levelGrid.GetTileAtGridPosition(stairDownPos);
                        if (stairDownTile != null && !stairDownTile.IsBlocked())
                        {
                            var stairDownDesc = stairDownTile.GetTileDescriptor();
                            if (!stairDownDesc.IsAirBlock && !stairDownDesc.IsFullBlock)
                                TryAdd(stairDownPos, currentCost + stairDownDesc.Cost);
                        }
                    }

                    // --- Movimiento normal sobre suelo ---
                    Vector3 nextGroundPos = nextAirPos + Vector3.down;
                    if (!levelGrid.IsGridPositionInBounds(nextGroundPos))
                        continue;

                    Tile3d nextGroundTile = levelGrid.GetTileAtGridPosition(nextGroundPos);
                    if (nextGroundTile == null)
                        continue;

                    var groundDesc = nextGroundTile.GetTileDescriptor();
                    if (groundDesc.IsAirBlock || groundDesc.IsFluidBlock)
                        continue;

                    DirectionFlag moveDir = DirectionHelper.GetDirectionFlag(possibleDirection);
                    if (nextAirTile.CanBeAccessedFrom(moveDir) && currentCost < movementRange)
                        TryAdd(nextAirPos, currentCost + groundDesc.Cost);
                }

                iterations++;
                if (iterations % yieldFrequency == 0)
                    await Task.Yield();
            }
        }
        public static List<Vector3> FindPath(Vector3 startGrid, Vector3 targetGrid, Grid3d levelGrid)
        {
            List<Node> openList = new List<Node>();
            HashSet<Vector3Int> closedList = new HashSet<Vector3Int>();

            Vector3Int start = Vector3Int.FloorToInt(startGrid);
            Vector3Int target = Vector3Int.FloorToInt(targetGrid);

            Node startNode = new Node(start, null, 0, Heuristic(start, target));
            openList.Add(startNode);


            while (openList.Count > 0)
            {
                Node currentNode = openList.OrderBy(n => n.f).First();
                openList.Remove(currentNode);
                closedList.Add(currentNode.gridPos);

                if (currentNode.gridPos == target)
                    return ReconstructPath(currentNode, levelGrid);

                foreach (Vector3Int neighbourPos in GetNeighbours(currentNode.gridPos, levelGrid))
                {
                    if (closedList.Contains(neighbourPos))
                        continue;

                    Tile3d tile = levelGrid.GetTileAtGridPosition(neighbourPos);
                    if (tile == null || tile.IsBlocked())
                        continue;

                    float tentativeG = currentNode.g + 1;

                    Node existingNode = openList.FirstOrDefault(n => n.gridPos == neighbourPos);
                    if (existingNode != null && tentativeG >= existingNode.g)
                        continue;

                    float h = Heuristic(neighbourPos, target);
                    Node neighbourNode = new Node(neighbourPos, currentNode, tentativeG, h);

                    if (existingNode != null)
                        openList.Remove(existingNode);

                    openList.Add(neighbourNode);
                }

            }
            Debug.Log("No se encontro un camino.");
            return null;
        }

        private static List<Vector3Int> GetNeighbours(Vector3Int currentAirPos, Grid3d levelGrid)
        {
            List<Vector3Int> neighbours = new();

            Tile3d currentAirTile = levelGrid.GetTileAtGridPosition(currentAirPos);
            if (currentAirTile == null) return neighbours;

            var currentDesc = currentAirTile.GetTileDescriptor();
            bool isOnStair = !currentDesc.IsAirBlock && !currentDesc.IsFullBlock;

            // ===== Caso: si estás encima de una escalera, subir según su dirección =====
            if (isOnStair)
            {
                foreach (var stairDir in currentAirTile.GetExitDirectionVector())
                {
                    Vector3Int stairTopPos = currentAirPos + Vector3Int.FloorToInt(stairDir);
                    if (!levelGrid.IsGridPositionInBounds(stairTopPos)) continue;

                    Tile3d stairTopTile = levelGrid.GetTileAtGridPosition(stairTopPos);
                    if (stairTopTile != null && !stairTopTile.IsBlocked())
                    {
                        neighbours.Add(stairTopPos);
                    }
                }
            }

            // ===== Iterar todas las direcciones de salida del tile actual =====
            foreach (var possibleDirection in currentAirTile.GetExitDirectionVector())
            {
                Vector3Int nextAirPos = currentAirPos + Vector3Int.FloorToInt(possibleDirection);
                if (!levelGrid.IsGridPositionInBounds(nextAirPos)) continue;

                Tile3d nextAirTile = levelGrid.GetTileAtGridPosition(nextAirPos);
                if (nextAirTile == null || nextAirTile.IsBlocked()) continue;

                var nextAirDesc = nextAirTile.GetTileDescriptor();

                // ===== Escalera: entrar o subir =====
                if (!nextAirDesc.IsAirBlock && !nextAirDesc.IsFullBlock && nextAirTile.CanBeAccessedFrom(possibleDirection))
                {
                    if (!neighbours.Contains(nextAirPos))
                    {
                        neighbours.Add(nextAirPos);

                        // Posición diagonal hacia arriba según la dirección de entrada
                        Vector3Int stairTopPos = nextAirPos + Vector3Int.FloorToInt(possibleDirection) + Vector3Int.up;
                        if (levelGrid.IsGridPositionInBounds(stairTopPos))
                        {
                            Tile3d stairTopTile = levelGrid.GetTileAtGridPosition(stairTopPos);
                            if (stairTopTile != null && !stairTopTile.IsBlocked())
                                neighbours.Add(stairTopPos);
                        }
                    }
                }

                // ===== Escalera: bajar =====
                Vector3Int stairDownPos = currentAirPos + Vector3Int.FloorToInt(possibleDirection) + Vector3Int.down;
                if (levelGrid.IsGridPositionInBounds(stairDownPos))
                {
                    Tile3d stairDownTile = levelGrid.GetTileAtGridPosition(stairDownPos);
                    if (stairDownTile != null && !stairDownTile.IsBlocked())
                    {
                        var stairDownDesc = stairDownTile.GetTileDescriptor();
                        if (!stairDownDesc.IsAirBlock && !stairDownDesc.IsFullBlock)
                            neighbours.Add(stairDownPos);
                    }
                }

                // ===== Movimiento normal sobre suelo =====
                Vector3Int nextGroundPos = nextAirPos + Vector3Int.down;
                if (!levelGrid.IsGridPositionInBounds(nextGroundPos)) continue;

                Tile3d nextGroundTile = levelGrid.GetTileAtGridPosition(nextGroundPos);
                if (nextGroundTile == null) continue;

                var groundDesc = nextGroundTile.GetTileDescriptor();
                if (groundDesc.IsAirBlock || groundDesc.IsFluidBlock) continue;

                DirectionFlag moveDir = DirectionHelper.GetDirectionFlag(possibleDirection);
                if (nextAirTile.CanBeAccessedFrom(moveDir))
                {
                    neighbours.Add(nextAirPos);
                }
            }

            return neighbours;
        }




        private static float Heuristic(Vector3Int a, Vector3Int b)
        {
            // Distancia Manhattan en grid 3D
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
        }

        private static List<Vector3> ReconstructPath(Node endNode, Grid3d levelGrid)
        {
            List<Vector3> path = new();
            Node current = endNode;

            while (current != null)
            {
                if (levelGrid.TryGridToWorldPosition(current.gridPos, out Vector3 worldPos))
                {
                    var offset = levelGrid.GetTileAtGridPosition(current.gridPos).GetDisplayOffset();
                    worldPos += new Vector3(0.5f, 0f, 0.5f) + offset;
                    path.Add(worldPos);
                }

                current = current.parent;
            }

            path.Reverse();
            return path;
        }

        private class Node
        {
            public Vector3Int gridPos;
            public Node parent;
            public float g, h, f;

            public Node(Vector3Int pos, Node parent, float g, float h)
            {
                this.gridPos = pos;
                this.parent = parent;
                this.g = g;
                this.h = h;
                this.f = g + h;
            }
        }
    }
}