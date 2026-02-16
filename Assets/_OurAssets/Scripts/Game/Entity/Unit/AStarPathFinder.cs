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

            public int version;
            public void Reset(Vector3Int gridPos, Node parent, float g, float h)
            {
                this.gridPosition = gridPos;
                this.parent = parent;
                this.g = g;
                this.h = h;
                this.version = 0;
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
        private struct NodeEntry
        {
            public Node node;
            public int version;
        }
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

        public static void InsertRangedAttackGridPositions(List<Vector3> reachableGridPositions, Grid3d grid,
            Vector3 startGridPos, int range)
        {
            reachableGridPositions.Clear();

            Vector3Int start = Vector3Int.FloorToInt(startGridPos);
            foreach (var direction in CentralXZ)
            {
                var gridPos = start + direction * range;
                if (!grid.TryGetTileAtGridPosition(gridPos, out Tile3d targetTile)) continue;
                
                TryToInsertGridPosition(reachableGridPositions, gridPos, targetTile, grid);
            }
        }

        public static void InsertManhattanAttackGridPositions(
            List<Vector3> reachableGridPositions,
            Grid3d grid,
            Vector3 startGridPos,
            int distance,
            bool fill,
            Func<Tile3d, bool> filter = null)
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
                    
                    if (!grid.TryGetTileAtGridPosition(targetPos, out Tile3d targetTile)) continue;
                    if (filter != null && !filter(targetTile)) continue;
                    
                    TryToInsertGridPosition(reachableGridPositions, targetPos, targetTile, grid);

                }
            }
        }

        static void TryToInsertGridPosition(List<Vector3> reachableGridPositions, Vector3Int targetPos, Tile3d targetTile, Grid3d grid)
        {
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
        
        private static void FillExitDirectionsAsInts(Tile3d tile, List<Vector3Int> outDirections)
        {
            outDirections.Clear();
            foreach (var v in tile.GetExitDirectionVector())
                outDirections.Add(Vector3Int.FloorToInt(v));
        }
        public static List<Vector3> FindPath(
            Vector3 startGridFloat,
            Vector3 targetGridFloat,
            Grid3d levelGrid,
            BattleSide side)
        {
            Vector3Int start = Vector3Int.FloorToInt(startGridFloat);
            Vector3Int target = Vector3Int.FloorToInt(targetGridFloat);
            
            if (!levelGrid.IsGridPositionInBounds(start)
                || !levelGrid.TryGetTileAtGridPosition(target, out Tile3d targetTile)
                || targetTile.IsBlocked())
                return null;
            
            var openQueue = new PriorityQueue<NodeEntry>();
            var openMap   = new Dictionary<Vector3Int, Node>();
            var closedSet = new HashSet<Vector3Int>();

            var startNode = RentNode(start, null, 0f, Heuristic(start, target));
            openMap[start] = startNode;

            openQueue.Enqueue(new NodeEntry
            {
                node = startNode,
                version = startNode.version
            }, startNode.f);

            List<Vector3Int> neighbourList = new List<Vector3Int>(8);
            
            const int MAX_NODES = 20000; //Máximo de nodos a comprobar (no debería nunca llegar al límite pero por si acaso)
            int processed = 0;
            
            while (openQueue.Count > 0)
            {
                processed++;
                if (processed > MAX_NODES)
                {
                    Debug.LogWarning("Pathfinding abortado: demasiados nodos explorados");
                    foreach (var kv in openMap)
                        ReturnNode(kv.Value);

                    return null;
                }

                var entry = openQueue.Dequeue();
                Node current = entry.node;

                // DESCARTAR versión antigua
                if (current.version != entry.version)
                    continue;

                // Mover a cerrado
                openMap.Remove(current.gridPosition);
                closedSet.Add(current.gridPosition);

                // Comprobar final
                if (current.gridPosition == target)
                {
                    var path = ReconstructPathNodes(current, levelGrid);

                    foreach (var kv in openMap)
                        ReturnNode(kv.Value);

                    ReturnNode(current);
                    return path;
                }

                // EXPANSIÓN DE NODOS
                neighbourList.Clear();
                FillNeighboursSafe(current.gridPosition, levelGrid, neighbourList, side);

                foreach (var neigh in neighbourList)
                {
                    if (closedSet.Contains(neigh))
                        continue;

                    // Costo acumulado
                    float tentativeG = current.g + 1f;

                    // Ya existente
                    if (openMap.TryGetValue(neigh, out Node existing))
                    {
                        if (tentativeG >= existing.g)
                            continue;

                        // Actualizar
                        existing.parent = current;
                        existing.g = tentativeG;
                        existing.version++;

                        openQueue.Enqueue(new NodeEntry
                        {
                            node = existing,
                            version = existing.version
                        }, existing.f);
                    }
                    else
                    {
                        // Nuevo nodo
                        float h = Heuristic(neigh, target);
                        Node nn = RentNode(neigh, current, tentativeG, h);

                        openMap[neigh] = nn;

                        openQueue.Enqueue(new NodeEntry
                        {
                            node = nn,
                            version = nn.version
                        }, nn.f);
                    }
                }
            }

            // No path
            foreach (var kv in openMap)
                ReturnNode(kv.Value);

            return null;
        }

        private static void FillNeighboursSafe(
            Vector3Int currentAirPos,
            Grid3d levelGrid,
            List<Vector3Int> neighbours,
            BattleSide side)
        {
            neighbours.Clear();

            Tile3d currentTile = levelGrid.GetTileAtGridPosition(currentAirPos);
            if (currentTile == null)
                return;

            Tile3dDescriptor currentDesc = currentTile.GetTileDescriptor();
            bool onStair = currentDesc.IsStairBlock;

            if (onStair)
            {
                foreach (var v in currentTile.GetExitDirectionVector())
                {
                    Vector3Int dir = Vector3Int.FloorToInt(v);
                    Vector3Int stairTop = currentAirPos + dir;

                    if (!levelGrid.IsGridPositionInBounds(stairTop))
                        continue;

                    if (!IsValidMove(levelGrid, stairTop, side))
                        continue;

                    if (stairTop != currentAirPos)
                        neighbours.Add(stairTop);
                }
            }
            
            foreach (var v in currentTile.GetExitDirectionVector())
            {
                Vector3Int dir = Vector3Int.FloorToInt(v);
                Vector3Int nextAirPos = currentAirPos + dir;

                if (!levelGrid.IsGridPositionInBounds(nextAirPos))
                    continue;

                Tile3d nextAirTile = levelGrid.GetTileAtGridPosition(nextAirPos);

                if (nextAirTile == null || nextAirTile.IsBlockedByEnemyOf(side))
                    continue;

                var nextDesc = nextAirTile.GetTileDescriptor();

                // SUBIR ESCALERA
                if (nextDesc.IsStairBlock &&
                    (nextAirTile.CanBeAccessedFrom(v) || nextAirTile.HasEntityWithSide(side)))
                {
                    if (nextAirPos != currentAirPos)
                        neighbours.Add(nextAirPos);

                    Vector3Int stairTop = nextAirPos + dir + Up;
                    if (IsValidMove(levelGrid, stairTop, side))
                        neighbours.Add(stairTop);
                }

                // BAJAR ESCALERA
                Vector3Int stairDown = currentAirPos + dir + Down;
                if (IsValidMove(levelGrid, stairDown, side))
                {
                    var desc = levelGrid.GetTileAtGridPosition(stairDown).GetTileDescriptor();
                    if (!desc.IsAirBlock && !desc.IsFullBlock)
                        neighbours.Add(stairDown);
                }

                // MOVIMIENTO HORIZONTAL NORMAL
                Vector3Int groundPos = nextAirPos + Down;

                if (!IsValidGround(levelGrid, groundPos))
                    continue;

                DirectionFlag moveDir = DirectionHelper.GetDirectionFlag(v);
                if (nextAirTile.CanBeAccessedFrom(moveDir) || nextAirTile.HasEntityWithSide(side))
                {
                    if (nextAirPos != currentAirPos)
                        neighbours.Add(nextAirPos);
                }
            }

            // Eliminar duplicados
            for (int i = neighbours.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < i; j++)
                {
                    if (neighbours[i] == neighbours[j])
                    {
                        neighbours.RemoveAt(i);
                        break;
                    }
                }
            }
        }
        private static bool IsValidMove(Grid3d grid, Vector3Int pos, BattleSide side)
        {
            if (!grid.IsGridPositionInBounds(pos))
                return false;

            Tile3d tile = grid.GetTileAtGridPosition(pos);
            return tile != null && !tile.IsBlockedByEnemyOf(side);
        }
        private static bool IsValidGround(Grid3d grid, Vector3Int pos)
        {
            if (!grid.IsGridPositionInBounds(pos))
                return false;

            Tile3d tile = grid.GetTileAtGridPosition(pos);
            if (tile == null)
                return false;

            var desc = tile.GetTileDescriptor();
            return !(desc.IsAirBlock || desc.IsFluidBlock);
        }
        
        private static float Heuristic(Vector3Int a, Vector3Int b)
        {
            return Vector3.Distance(a, b) * 1.15f;
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