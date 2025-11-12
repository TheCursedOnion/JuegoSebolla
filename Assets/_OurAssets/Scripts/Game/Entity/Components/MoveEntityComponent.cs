using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CursedOnion.Extensions;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.Game.Systems.Level;
using CursedOnion.Helpers;
using UnityEngine;

namespace CursedOnion.Game.Entity.Components
{
    public class EntityComponent
    {
        protected SimpleEntity AssignedEntity;
        protected Transform EntityTransform => AssignedEntity.transform;

        public virtual void ConfigureComponent(SimpleEntity assignedEntity)
        {
            AssignedEntity = assignedEntity;
        }
        public void ProcessTurn()
        {
            
        }
    }

    [System.Serializable]
    public class MoveEntityComponent : EntityComponent
    {
        [SerializeField] protected Color movementColor = Color.blue;
        
        [SerializeReference, SubclassSelector] protected AStarPathFinder PathFinder = new AStarPathFinder();
        public AStarPathFinder GetPathFinder() => PathFinder;
        
        private static List<Vector3> previousReachablePositions = new();
        private Vector3 lastTargetPosition;
        public override void ConfigureComponent(SimpleEntity assignedEntity)
        {
            base.ConfigureComponent(assignedEntity);
            lastTargetPosition = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
        }
        
        public virtual async Task<List<Vector3>> GetReachablePositionsMovementAsync(Grid3d levelGrid, Vector3 startWorldPos, int movementRange, int yieldFrequency = 100)
        {
            if (Vector3.Distance(lastTargetPosition, startWorldPos) < 0.1f)
                return previousReachablePositions;
            
            if (!levelGrid.TryWorldToGridPosition(startWorldPos, out Vector3 startGrid))
                return null;
            
            
            
            
            Vector3Int start = new Vector3Int(
                Mathf.FloorToInt(startGrid.x),
                Mathf.FloorToInt(startGrid.y),
                Mathf.FloorToInt(startGrid.z)
            );
            
            var frontier = new Queue<(Vector3Int pos, int cost)>();
            var visited = new HashSet<Vector3Int>();

            frontier.Enqueue((start, 0));
            visited.Add(start);

            Vector3Int[] directions =
            {
                new Vector3Int(1, 0, 0),
                new Vector3Int(-1, 0, 0),
                new Vector3Int(0, 0, 1),
                new Vector3Int(0, 0, -1)
            };

            int iterations = 0;
            previousReachablePositions.Clear();
            while (frontier.Count > 0)
            {
                var (currentAirPos, cost) = frontier.Dequeue();

                if (cost > 0)
                    previousReachablePositions.Add(currentAirPos);

                if (cost >= movementRange)
                    continue;

                Vector3Int currentGroundPos = currentAirPos + Vector3Int.down;
                Tile3d groundTile = levelGrid.GetTileAtGridPosition(currentGroundPos);
                if (groundTile == null)
                    continue;

                foreach (var dir in directions)
                {
                    Vector3Int nextAirPos = currentAirPos + dir;
                    if (!levelGrid.IsGridPositionInBounds(nextAirPos))
                        continue;

                    Tile3d nextAirTile = levelGrid.GetTileAtGridPosition(nextAirPos);
                    if (nextAirTile.GetContainedEntity() != null)
                        continue;

                    if (visited.Contains(nextAirPos))
                        continue;

                    Vector3Int nextGroundPos = nextAirPos + Vector3Int.down;
                    if (!levelGrid.IsGridPositionInBounds(nextGroundPos))
                        continue;

                    Tile3d nextGroundTile = levelGrid.GetTileAtGridPosition(nextGroundPos);
                    if (nextGroundTile == null)
                        continue;

                    var nextDesc = nextGroundTile.GetTileDescriptor();
                    if (nextDesc.IsAirBlock)
                        continue;

                    DirectionFlag moveDir = DirectionHelper.GetDirectionFlag(dir);
                    DirectionFlag opposite = DirectionHelper.GetDirectionFlag(-dir);

                    if ((groundTile.GetExitDirections() & moveDir) != 0 &&
                        (nextGroundTile.GetEntryDirections() & opposite) != 0)
                    {
                        frontier.Enqueue((nextAirPos, cost + nextDesc.Cost + 1));
                        visited.Add(nextAirPos);
                    }
                }
                
                iterations++;
                if (iterations % yieldFrequency == 0)
                    await Task.Yield();
            }
            
            
            lastTargetPosition = startWorldPos;
            return previousReachablePositions;
        }

        
        public virtual async void VisualizeMovement()
        {
            int moveRange = AssignedEntity.GetStats().MovementStat;
            
            var reachablePositions = await GetReachablePositionsMovementAsync(AssignedEntity.Grid, EntityTransform.position, moveRange);
            
            AssignedEntity.LevelManager.LevelAsset.Grid.PaintTilesAtGridPositions(reachablePositions, movementColor);
        }
        public virtual void DoMove(Vector3 newPosition, bool undo)
        {
            if(AssignedEntity == null) return;

            var grid = AssignedEntity.Grid;
            var transform = EntityTransform;
            
            if (undo)
            {
                transform.position = newPosition;
            }
            else
            {
                if (!AssignedEntity.Grid.TryWorldToGridPosition(transform.position, out Vector3 startGrid))
                {
                    return;
                }
                
                grid.ResetPaint();
                var path = GetPathFinder().FindPath(startGrid, newPosition, grid);
                if (path == null || path.Count == 0)
                {
                    Debug.LogWarning("No se encontró camino (FindPath devolvió null/empty).");
                    return;
                }
                
                if(AssignedEntity.TryGetLayeredEntity(out var layeredEntity)) layeredEntity.PlayAnimation("walk");
                
                AssignedEntity.StartCoroutine(MoveAlongPath(path));
            }
        }

        public virtual async Task<bool> ValidateMove(Vector3 newPosition)
        {
            AssignedEntity.Grid.ResetPaint();
            
            int moveRange = AssignedEntity.GetStats().MovementStat;
            var reachable = await GetReachablePositionsMovementAsync(AssignedEntity.Grid, EntityTransform.position, moveRange);
            Vector3Int target = newPosition.CastToVectorInt();
            
            return reachable.Contains(target);
        }
        private IEnumerator MoveAlongPath(List<Vector3> path)
        {
            var transform = EntityTransform;
            
            AssignedEntity.Grid.GetTileAtWorldPosition(EntityTransform.position).SetContainedEntity(null);

            float speed = 5f;
            Vector3 lastPosition = transform.position;

            foreach (var pos in path)
            {
                Vector3 direction = pos - lastPosition;

                if (direction.x > 0.01f)
                {
                    Vector3 scale = transform.localScale;
                    scale.x = Mathf.Abs(scale.x) * -1f;
                    transform.localScale = scale;
                }
                else if (direction.x < -0.01f)
                {
                    Vector3 scale = transform.localScale;
                    scale.x = Mathf.Abs(scale.x);
                    transform.localScale = scale;
                }

                while (Vector3.Distance(transform.position, pos) > 0.01f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, pos, speed * Time.deltaTime);
                    yield return null;
                }

                transform.position = pos;
                lastPosition = pos;
                yield return null;
            }
            
            if(AssignedEntity.TryGetLayeredEntity(out var layeredEntity)) layeredEntity.PlayAnimation("idle");
            AssignedEntity.Grid.GetTileAtWorldPosition(transform.position).SetContainedEntity(AssignedEntity);
        }
    }
}
