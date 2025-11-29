using System;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace CursedOnion.Game.Entity
{
    public static class AStarPathFinder
    {
        private static readonly Vector3Int[] CentralXZ =
        {
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 0, 1),
            new Vector3Int(0, 0, -1),
        };

        private static readonly Vector3Int Up = Vector3Int.up;
        private static readonly Vector3Int Down = Vector3Int.down;

        #region Node

        private class Node
        {
            public Vector3Int gridPosition;
            public Node parent;
            public float g;
            public float h;
            public float f => g + h;

            public void Reset(Vector3Int gridPos, Node parent, float g, float h)
            {
                this.gridPosition = gridPos;
                this.parent = parent;
                this.g = g;
                this.h = h;
            }
        }

        private static readonly Stack<Node> NodePool = new();

        private static Node RentNode(Vector3Int pos, Node parent, float g, float h)
        {
            Node n;
            if (NodePool.Count > 0)
                n = NodePool.Pop();
            else
                n = new Node();

            n.gridPosition = pos;
            n.parent = parent;
            n.g = g;
            n.h = h;

            return n;
        }

        private static void ReturnNode(Node n)
        {
            if (n == null) return;

            n.parent = null;
            n.g = 0f;
            n.h = 0f;

            NodePool.Push(n);
        }


        #endregion

        #region Priority Queue
        class PriorityQueue<T>
        {
            private readonly List<(T item, float priority)> heap = new();

            public int Count => heap.Count;

            public void Enqueue(T item, float priority)
            {
                heap.Add((item, priority));
                SiftUp(heap.Count - 1);
            }

            public T Dequeue()
            {
                if (heap.Count == 0) throw new InvalidOperationException("Queue empty");
                var result = heap[0].item;
                if (heap.Count == 1)
                {
                    heap.RemoveAt(0);
                    return result;
                }

                heap[0] = heap[heap.Count - 1];
                heap.RemoveAt(heap.Count - 1);
                SiftDown(0);
                return result;
            }

            public void Clear() => heap.Clear();

            private void SiftUp(int index)
            {
                while (index > 0)
                {
                    int parent = (index - 1) / 2;
                    if (heap[index].priority >= heap[parent].priority) break;
                    Swap(index, parent);
                    index = parent;
                }
            }

            private void SiftDown(int index)
            {
                int n = heap.Count;
                while (true)
                {
                    int left = 2 * index + 1;
                    int right = left + 1;
                    int smallest = index;

                    if (left < n && heap[left].priority < heap[smallest].priority) smallest = left;
                    if (right < n && heap[right].priority < heap[smallest].priority) smallest = right;

                    if (smallest == index) break;
                    Swap(index, smallest);
                    index = smallest;
                }
            }

            private void Swap(int a, int b)
            {
                (heap[a], heap[b]) = (heap[b], heap[a]);
            }
        }

        #endregion

        #region Insert Attack Methods

        public static void InsertMeleeAttackGridPositions(List<Vector3> reachableGridPositions, Grid3d grid,
            Vector3 startGridPos)
        {
            reachableGridPositions.Clear();

            Vector3Int start = Vector3Int.FloorToInt(startGridPos);
            Tile3d tile = grid.GetTileAtGridPosition(start);
            if (tile == null) return;

            foreach (var dirV in tile.GetExitDirectionVector())
            {
                Vector3Int dir = Vector3Int.FloorToInt(dirV);
                Vector3Int newPos = start + dir;
                if (grid.TryGetTileAtGridPosition(newPos, out var nextTile) && !nextTile.IsFullTile())
                {
                    reachableGridPositions.Add(newPos);
                }
            }
        }

        public static void InsertRangedAttackGridPositions(List<Vector3> reachableGridPositions, Grid3d grid,
            Vector3 startGridPos, int range)
        {
            reachableGridPositions.Clear();

            Vector3Int start = Vector3Int.FloorToInt(startGridPos);
            foreach (var direction in CentralXZ)
            {
                var gridPos = start + direction * range;
                TryToInsertGridPosition(reachableGridPositions, gridPos, grid);
            }
        }

        public static void InsertManhattanAttackGridPositions(
            List<Vector3> reachableGridPositions,
            Grid3d grid,
            Vector3 startGridPos,
            int distance,
            bool fill)
        {
            reachableGridPositions.Clear();

            Vector3Int start = Vector3Int.FloorToInt(startGridPos);

            for (int dx = -distance; dx <= distance; dx++)
            {
                int maxDz = distance - Mathf.Abs(dx);

                for (int dz = -maxDz; dz <= maxDz; dz++)
                {
                    int manhattan = Mathf.Abs(dx) + Mathf.Abs(dz);

                    if (!fill && manhattan != distance) continue;

                    Vector3Int targetPos = new Vector3Int(start.x + dx, start.y, start.z + dz);
                    TryToInsertGridPosition(reachableGridPositions, targetPos, grid);

                }
            }
        }

        static void TryToInsertGridPosition(List<Vector3> reachableGridPositions, Vector3Int targetPos, Grid3d grid)
        {
            if (!grid.TryGetTileAtGridPosition(targetPos, out Tile3d targetTile)) return;

            bool isTargetFull = targetTile.IsFullTile();
            bool isTargetEmpty = targetTile.IsEmptyTile();
            bool isTargetStair = targetTile.IsStairTile();

            if (isTargetFull)
            {
                if (grid.TryGetTileAtGridPosition(targetPos + Up, out Tile3d above) && above.IsEmptyTile())
                {
                    reachableGridPositions.Add(targetPos + Up);
                }

                return;
            }

            if (isTargetStair)
            {
                reachableGridPositions.Add(targetPos);
                return;
            }

            if (isTargetEmpty)
            {
                Vector3Int belowPos = targetPos + Down;

                if (grid.TryGetTileAtGridPosition(belowPos, out Tile3d below))
                {
                    if (below.IsStairTile())
                    {
                        reachableGridPositions.Add(targetPos + Down);
                        return;
                    }

                    if (below.IsFullTile())
                    {
                        reachableGridPositions.Add(targetPos);
                        return;
                    }

                    Vector3Int lastBelowPos = targetPos + Down * 2;
                    if (grid.TryGetTileAtGridPosition(lastBelowPos, out Tile3d lastBelow)
                        && lastBelow.IsFullTile())
                    {
                        reachableGridPositions.Add(targetPos + Down);
                    }
                }
            }
        }

        #endregion

        #region Pathfinding Methods

        public static async Task InsertReachableGridPositionsAsyncBFS(
            List<Vector3> positions,
            Grid3d levelGrid,
            BattleSide side,
            Vector3 startWorldPos,
            int movementRange,
            int yieldFrequency = 100)
        {
            void TryAdd(Vector3 position)
            {
                if(!positions.Contains(position)) positions.Add(position);
            }
            if (!levelGrid.TryWorldToGridPosition(startWorldPos, out Vector3 startGridFloat))
                return;

            Vector3Int start = Vector3Int.FloorToInt(startGridFloat);

            positions.Clear();

            var frontier = new Queue<(Vector3Int pos, int cost)>();
            var costSoFar = new Dictionary<Vector3Int, int>();

            frontier.Enqueue((start, 0));
            costSoFar[start] = 0;

            int iterations = 0;

            var neighbours = new List<Vector3Int>(8);

            while (frontier.Count > 0)
            {
                var (currentPos, currentCost) = frontier.Dequeue();

                if (!levelGrid.IsGridPositionInBounds(currentPos))
                    continue;

                Tile3d currentTile = levelGrid.GetTileAtGridPosition(currentPos);
                if (currentTile == null) continue;

                var currentDesc = currentTile.GetTileDescriptor();
                bool isOnStair = currentDesc.IsStairBlock;

                // Si es escalera, moverse según salidas
                if (isOnStair)
                {
                    neighbours.Clear();
                    FillExitDirectionsAsInts(currentTile, neighbours);
                    foreach (var stairDir in neighbours)
                    {
                        Vector3Int stairTopPos = currentPos + stairDir;
                        if (!levelGrid.IsGridPositionInBounds(stairTopPos)) continue;

                        Tile3d stairTopTile = levelGrid.GetTileAtGridPosition(stairTopPos);
                        if (stairTopTile != null && !stairTopTile.IsBlockedByEnemyOf(side))
                        {
                            int newCost = currentCost + Mathf.RoundToInt(currentDesc.Cost);
                            if (newCost <= movementRange &&
                                (!costSoFar.ContainsKey(stairTopPos) || newCost < costSoFar[stairTopPos]))
                            {
                                costSoFar[stairTopPos] = newCost;
                                frontier.Enqueue((stairTopPos, newCost));
                                TryAdd(stairTopPos);
                            }
                        }
                    }
                }

                // Movimiento normal usando salidas de la tile
                neighbours.Clear();
                FillExitDirectionsAsInts(currentTile, neighbours);

                foreach (var dir in neighbours)
                {
                    Vector3Int nextAirPos = currentPos + dir;
                    if (!levelGrid.IsGridPositionInBounds(nextAirPos)) continue;

                    Tile3d nextAirTile = levelGrid.GetTileAtGridPosition(nextAirPos);
                    if (nextAirTile == null || nextAirTile.IsBlockedByEnemyOf(side)) continue;

                    var nextAirDesc = nextAirTile.GetTileDescriptor();

                    // Subir a escalera
                    if (nextAirDesc.IsStairBlock && (nextAirTile.CanBeAccessedFrom(dir) || nextAirTile.HasEntityWithSide(side)))
                    {
                        int newCost = currentCost + Mathf.RoundToInt(nextAirDesc.Cost);
                        if (newCost <= movementRange &&
                            (!costSoFar.ContainsKey(nextAirPos) || newCost < costSoFar[nextAirPos]))
                        {
                            costSoFar[nextAirPos] = newCost;
                            frontier.Enqueue((nextAirPos, newCost));
                            TryAdd(nextAirPos);
                        }

                        // También intentar la posición superior de la escalera
                        Vector3Int stairTopPos = nextAirPos + dir + Up;
                        if (levelGrid.IsGridPositionInBounds(stairTopPos))
                        {
                            Tile3d stairTopTile = levelGrid.GetTileAtGridPosition(stairTopPos);
                            if (stairTopTile != null && !stairTopTile.IsBlockedByEnemyOf(side))
                            {
                                int newCostTop = currentCost + Mathf.RoundToInt(nextAirDesc.Cost);
                                if (newCostTop <= movementRange && (!costSoFar.ContainsKey(stairTopPos) ||
                                                                    newCostTop < costSoFar[stairTopPos]))
                                {
                                    costSoFar[stairTopPos] = newCostTop;
                                    frontier.Enqueue((stairTopPos, newCostTop));
                                    TryAdd(stairTopPos);
                                }
                            }
                        }
                    }

                    // Bajar escalera (pos actual + dir + down)
                    Vector3Int stairDownPos = currentPos + dir + Down;
                    if (levelGrid.IsGridPositionInBounds(stairDownPos))
                    {
                        Tile3d stairDownTile = levelGrid.GetTileAtGridPosition(stairDownPos);
                        if (stairDownTile != null && !stairDownTile.IsBlockedByEnemyOf(side))
                        {
                            var stairDownDesc = stairDownTile.GetTileDescriptor();
                            if (!stairDownDesc.IsAirBlock && !stairDownDesc.IsFullBlock)
                            {
                                int newCost = currentCost + Mathf.RoundToInt(stairDownDesc.Cost);
                                if (newCost <= movementRange && (!costSoFar.ContainsKey(stairDownPos) ||
                                                                 newCost < costSoFar[stairDownPos]))
                                {
                                    costSoFar[stairDownPos] = newCost;
                                    frontier.Enqueue((stairDownPos, newCost));
                                    TryAdd(stairDownPos);
                                }
                            }
                        }
                    }

                    // Movimiento normal sobre suelo
                    Vector3Int nextGroundPos = nextAirPos + Down;
                    if (!levelGrid.IsGridPositionInBounds(nextGroundPos)) continue;

                    Tile3d nextGroundTile = levelGrid.GetTileAtGridPosition(nextGroundPos);
                    if (nextGroundTile == null) continue;

                    var groundDesc = nextGroundTile.GetTileDescriptor();
                    if (groundDesc.IsAirBlock || groundDesc.IsFluidBlock) continue;

                    DirectionFlag moveDir = DirectionHelper.GetDirectionFlag((Vector3)dir);
                    if (nextAirTile.CanBeAccessedFrom(moveDir) || nextAirTile.HasEntityWithSide(side))
                    {
                        int newCost = currentCost + Mathf.RoundToInt(groundDesc.Cost);
                        if (newCost <= movementRange &&
                            (!costSoFar.ContainsKey(nextAirPos) || newCost < costSoFar[nextAirPos]))
                        {
                            costSoFar[nextAirPos] = newCost;
                            frontier.Enqueue((nextAirPos, newCost));
                            TryAdd(nextAirPos);
                        }
                    }
                }

                iterations++;
                if (iterations % yieldFrequency == 0)
                    await Task.Yield();
            }
        }

        public static List<Vector3> FindPath(Vector3 startGridFloat, Vector3 targetGridFloat, Grid3d levelGrid, BattleSide side)
        {
            Vector3Int start = Vector3Int.FloorToInt(startGridFloat);
            Vector3Int target = Vector3Int.FloorToInt(targetGridFloat);

            if (!levelGrid.IsGridPositionInBounds(start)
                || !levelGrid.TryGetTileAtGridPosition(target, out Tile3d targetTile)
                || targetTile.IsBlocked())
                return null;

            var openQueue = new PriorityQueue<Node>();
            var openMap = new Dictionary<Vector3Int, Node>();
            var closedSet = new HashSet<Vector3Int>();
            var closedCosts = new Dictionary<Vector3Int, float>();

            var startNode = RentNode(start, null, 0f, Heuristic(start, target));
            openQueue.Enqueue(startNode, startNode.f);
            openMap[start] = startNode;

            var neighbourList = new List<Vector3Int>(8);

            while (openQueue.Count > 0)
            {
                var current = openQueue.Dequeue();

                if (!openMap.TryGetValue(current.gridPosition, out var mapped) || mapped != current)
                {
                    // Nodo antiguo; devolver al pool y continuar
                    ReturnNode(current);
                    continue;
                }

                // Remover del mapa abierto porque ahora está en cerrado
                openMap.Remove(current.gridPosition);
                closedSet.Add(current.gridPosition);
                closedCosts[current.gridPosition] = current.g;

                if (current.gridPosition == target)
                {
                    var path = ReconstructPathNodes(current, levelGrid);
                    foreach (var kv in openMap)
                        ReturnNode(kv.Value);
                    ReturnNode(current);
                    return path;
                }

                neighbourList.Clear();
                FillNeighbours(current.gridPosition, levelGrid, neighbourList, side);

                foreach (var neigh in neighbourList)
                {
                    Tile3d tile = levelGrid.GetTileAtGridPosition(neigh);
                    if (tile == null || tile.IsBlockedByEnemyOf(side)) continue;

                    float tentativeG = current.g + 1f;

                    if (closedCosts.ContainsKey(neigh))
                        continue;

                    if (closedSet.Contains(neigh))
                        continue;
                    
                    if (openMap.TryGetValue(neigh, out var existingOpen))
                    {
                        if (tentativeG >= existingOpen.g) continue;
                        
                        if (IsDescendant(current, existingOpen)) continue;

                        ReturnNode(existingOpen); // liberamos la instancia antigua limpiando parent
                        var replacement = RentNode(neigh, current, tentativeG, existingOpen.h); // conservamos h si lo prefieres
                        openMap[neigh] = replacement;

                        openQueue.Enqueue(existingOpen, existingOpen.f);
                    }
                    else
                    {
                        float h = Heuristic(neigh, target);
                        var newNode = RentNode(neigh, current, tentativeG, h);
                        openMap[neigh] = newNode;
                        openQueue.Enqueue(newNode, newNode.f);
                    }
                }
            }

            foreach (var kv in openMap) ReturnNode(kv.Value);

            return null;
        }
        private static bool IsDescendant(Node parentCandidate, Node possibleChild)
        {
            var p = parentCandidate;
            var visited = new HashSet<Node>();

            while (p != null)
            {
                if (!visited.Add(p))
                {
                    return false;
                }

                if (p == possibleChild) 
                    return true;

                p = p.parent;
            }
            return false;

        }

        private static void FillNeighbours(Vector3Int currentAirPos, Grid3d levelGrid, List<Vector3Int> neighbours, BattleSide side)
        {
            neighbours.Clear();

            Tile3d currentAirTile = levelGrid.GetTileAtGridPosition(currentAirPos);
            if (currentAirTile == null) return;

            var currentDesc = currentAirTile.GetTileDescriptor();
            bool isOnStair = currentDesc.IsStairBlock;

            if (isOnStair)
            {
                var exits = currentAirTile.GetExitDirectionVector();
                foreach (var v in exits)
                {
                    Vector3Int dir = Vector3Int.FloorToInt(v);
                    Vector3Int stairTopPos = currentAirPos + dir;
                    if (!levelGrid.IsGridPositionInBounds(stairTopPos)) continue;
                    Tile3d stairTopTile = levelGrid.GetTileAtGridPosition(stairTopPos);
                    if (stairTopTile != null && !stairTopTile.IsBlockedByEnemyOf(side)) neighbours.Add(stairTopPos);
                }
            }

            var exitDirs = currentAirTile.GetExitDirectionVector();
            foreach (var possibleDirection in exitDirs)
            {
                Vector3Int dir = Vector3Int.FloorToInt(possibleDirection);
                Vector3Int nextAirPos = currentAirPos + dir;
                if (!levelGrid.IsGridPositionInBounds(nextAirPos)) continue;

                Tile3d nextAirTile = levelGrid.GetTileAtGridPosition(nextAirPos);
                if (nextAirTile == null || nextAirTile.IsBlockedByEnemyOf(side)) continue;

                var nextAirDesc = nextAirTile.GetTileDescriptor();

                if (nextAirDesc.IsStairBlock && (nextAirTile.CanBeAccessedFrom(possibleDirection) || nextAirTile.HasEntityWithSide(side)))
                {
                    if (!neighbours.Contains(nextAirPos)) neighbours.Add(nextAirPos);

                    Vector3Int stairTopPos = nextAirPos + dir + Up;
                    if (levelGrid.IsGridPositionInBounds(stairTopPos))
                    {
                        Tile3d stairTopTile = levelGrid.GetTileAtGridPosition(stairTopPos);
                        if (stairTopTile != null && !stairTopTile.IsBlockedByEnemyOf(side)) neighbours.Add(stairTopPos);
                    }
                }

                Vector3Int stairDownPos = currentAirPos + dir + Down;
                if (levelGrid.IsGridPositionInBounds(stairDownPos))
                {
                    Tile3d stairDownTile = levelGrid.GetTileAtGridPosition(stairDownPos);
                    if (stairDownTile != null && !stairDownTile.IsBlockedByEnemyOf(side))
                    {
                        var stairDownDesc = stairDownTile.GetTileDescriptor();
                        if (!stairDownDesc.IsAirBlock && !stairDownDesc.IsFullBlock)
                            neighbours.Add(stairDownPos);
                    }
                }

                Vector3Int nextGroundPos = nextAirPos + Down;
                if (!levelGrid.IsGridPositionInBounds(nextGroundPos)) continue;

                Tile3d nextGroundTile = levelGrid.GetTileAtGridPosition(nextGroundPos);
                if (nextGroundTile == null) continue;

                var groundDesc = nextGroundTile.GetTileDescriptor();
                if (groundDesc.IsAirBlock || groundDesc.IsFluidBlock) continue;

                DirectionFlag moveDir = DirectionHelper.GetDirectionFlag(possibleDirection);
                if (nextAirTile.CanBeAccessedFrom(moveDir) || nextAirTile.HasEntityWithSide(side)) neighbours.Add(nextAirPos);
            }
        }

        private static void FillExitDirectionsAsInts(Tile3d tile, List<Vector3Int> outDirections)
        {
            outDirections.Clear();
            var exits = tile.GetExitDirectionVector();
            foreach (var v in exits)
                outDirections.Add(Vector3Int.FloorToInt(v));
        }

        private static float Heuristic(Vector3Int a, Vector3Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
        }

        private static List<Vector3> ReconstructPathNodes(Node endNode, Grid3d levelGrid)
        {
            var path = new List<Vector3>();
            var stackNodes = new Stack<Node>();
            var visited = new HashSet<Node>();

            Node current = endNode;
            
            while (current != null)
            {
                if (!visited.Add(current))
                {
                    Debug.LogError("Ciclo detectado en la cadena de nodos del pathfinding");
                    break;
                }
                
                stackNodes.Push(current);
                current = current.parent;
            }

            while (stackNodes.Count > 0)
            {
                var n = stackNodes.Pop();
                if (levelGrid.TryGridToWorldPosition(n.gridPosition, out Vector3 world))
                {
                    Tile3d t = levelGrid.GetTileAtGridPosition(n.gridPosition);
                    var offset = t != null ? t.GetDisplayOffset() : Vector3.zero;
                    path.Add(world + new Vector3(0.5f, 0f, 0.5f) + offset);
                }
            }

            return path;
        }

        #endregion
    }
}