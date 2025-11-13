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
        public static void InsertActionRange(List<Vector3> reachablePositions, Grid3d grid, Vector3 startGridPos)
        {
            Vector3[] directions =
            {
                new(1, 0, 0),
                new(-1, 0, 0),
                new(0, 0, 1),
                new(0, 0, -1)
            };
            reachablePositions.Clear();
            foreach (var direction in directions)
            {
                var newPos = startGridPos + direction;

                if (grid.GetTileAtGridPosition(newPos).IsEmptyTile())
                    reachablePositions.Add(startGridPos + direction);
            }
        }

        public static async Task InsertReachablePositionsAsyncBFS(List<Vector3> positions, Grid3d levelGrid, Vector3 startWorldPos, int movementRange, int yieldFrequency = 100)
        {
            if (!levelGrid.TryWorldToGridPosition(startWorldPos, out Vector3 startGrid)) return;

            Debug.Log(startGrid);

            var frontier = new Queue<(Vector3 pos, int cost)>();
            var visited = new HashSet<Vector3>();

            frontier.Enqueue((startGrid, 0));
            visited.Add(startGrid);

            int iterations = 0;
            positions.Clear();

            Vector3 currentAirPos;
            int currentCost;

            while (frontier.Count > 0)
            {
                currentAirPos = frontier.Peek().pos;
                currentCost = frontier.Dequeue().cost;
                if (!levelGrid.IsGridPositionInBounds(currentAirPos)) continue;
                Tile3d currentAirTile = levelGrid.GetTileAtGridPosition(currentAirPos);

                foreach (var possibleDirection in currentAirTile.GetExitDirectionVector())
                {
                    Vector3 nextAirPos = currentAirPos + possibleDirection;
                    if (!levelGrid.IsGridPositionInBounds(nextAirPos)) continue;
                    Tile3d nextAirTile = levelGrid.GetTileAtGridPosition(nextAirPos);

                    if (nextAirTile == null || nextAirTile.IsBlocked() || visited.Contains(nextAirPos)) continue;

                    var nextAirDescriptor = nextAirTile.GetTileDescriptor();

                    if (!nextAirDescriptor.IsAirBlock && !nextAirDescriptor.IsFullBlock)
                    {
                        // Caso: escalera desde abajo (subir)
                        Vector3 stairPos = nextAirPos;
                        if (!levelGrid.IsGridPositionInBounds(stairPos)) continue;

                        Tile3d stairTile = levelGrid.GetTileAtGridPosition(stairPos);
                        if (stairTile == null || stairTile.IsBlocked()) continue;

                        var stairDesc = stairTile.GetTileDescriptor();

                        if (!visited.Contains(stairPos) && nextAirTile.CanBeAccessedFrom(possibleDirection))
                        {
                            frontier.Enqueue((stairPos, currentCost + nextAirDescriptor.Cost));
                            positions.Add(stairPos);
                            visited.Add(stairPos);

                            // Posición diagonal hacia arriba según la dirección de entrada
                            Vector3 stairTopPos = stairPos + possibleDirection + Vector3.up;

                            if (levelGrid.IsGridPositionInBounds(stairTopPos))
                            {
                                Tile3d stairTopTile = levelGrid.GetTileAtGridPosition(stairTopPos);
                                if (stairTopTile != null && !stairTopTile.IsBlocked() && !visited.Contains(stairTopPos))
                                {
                                    frontier.Enqueue((stairTopPos, currentCost + stairDesc.Cost));
                                    positions.Add(stairTopPos);
                                    visited.Add(stairTopPos);
                                }
                            }
                        }
                    }

                    // Caso: escalera desde arriba (bajar)
                    Vector3 stairDownPos = currentAirPos + possibleDirection + Vector3.down;
                    if (levelGrid.IsGridPositionInBounds(stairDownPos))
                    {
                        Tile3d stairDownTile = levelGrid.GetTileAtGridPosition(stairDownPos);
                        if (stairDownTile != null && !stairDownTile.IsBlocked() && !visited.Contains(stairDownPos))
                        {
                            var stairDownDesc = stairDownTile.GetTileDescriptor();
                            if (!stairDownDesc.IsAirBlock && !stairDownDesc.IsFullBlock)
                            {
                                frontier.Enqueue((stairDownPos, currentCost + stairDownDesc.Cost));
                                positions.Add(stairDownPos);
                                visited.Add(stairDownPos);
                            }
                        }
                    }

                    // Caso: movimiento normal (suelo)
                    Vector3 nextGroundPos = nextAirPos + Vector3.down;
                    if (!levelGrid.IsGridPositionInBounds(nextGroundPos)) continue;

                    Tile3d nextGroundTile = levelGrid.GetTileAtGridPosition(nextGroundPos);
                    if (nextGroundTile == null) continue;

                    var groundDesc = nextGroundTile.GetTileDescriptor();
                    if (groundDesc.IsAirBlock || groundDesc.IsFluidBlock) continue;

                    DirectionFlag moveDir = DirectionHelper.GetDirectionFlag(possibleDirection);
                    if (nextAirTile.CanBeAccessedFrom(moveDir) && currentCost <= movementRange)
                    {
                        frontier.Enqueue((nextAirPos, currentCost + groundDesc.Cost));
                        positions.Add(nextAirPos);
                        visited.Add(nextAirPos);
                    }
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
            if (currentAirTile == null)
                return neighbours;

            foreach (var possibleDirection in currentAirTile.GetExitDirectionVector())
            {
                Vector3Int nextAirPos = currentAirPos + Vector3Int.FloorToInt(possibleDirection);
                if (!levelGrid.IsGridPositionInBounds(nextAirPos)) continue;

                Tile3d nextAirTile = levelGrid.GetTileAtGridPosition(nextAirPos);
                if (nextAirTile == null || nextAirTile.IsBlocked()) continue;

                var nextAirDesc = nextAirTile.GetTileDescriptor();

                // ===== ESCALERA: Subir =====
                if (!nextAirDesc.IsAirBlock && !nextAirDesc.IsFullBlock)
                {
                    Vector3Int stairPos = nextAirPos;

                    if (!neighbours.Contains(stairPos) && nextAirTile.CanBeAccessedFrom(possibleDirection))
                    {
                        neighbours.Add(stairPos);

                        // Posición diagonal hacia arriba según la dirección de entrada
                        Vector3Int stairTopPos = stairPos + Vector3Int.FloorToInt(possibleDirection) + Vector3Int.up;

                        if (levelGrid.IsGridPositionInBounds(stairTopPos))
                        {
                            Tile3d stairTopTile = levelGrid.GetTileAtGridPosition(stairTopPos);
                            if (stairTopTile != null && !stairTopTile.IsBlocked())
                                neighbours.Add(stairTopPos);
                        }
                    }
                }

                // ===== ESCALERA: Bajar =====
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

                // ===== SUELO normal =====
                Vector3Int nextGroundPos = nextAirPos + Vector3Int.down;
                if (!levelGrid.IsGridPositionInBounds(nextGroundPos)) continue;

                Tile3d nextGroundTile = levelGrid.GetTileAtGridPosition(nextGroundPos);
                if (nextGroundTile == null) continue;

                var groundDesc = nextGroundTile.GetTileDescriptor();
                if (groundDesc.IsAirBlock || groundDesc.IsFluidBlock) continue; // no hay suelo

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