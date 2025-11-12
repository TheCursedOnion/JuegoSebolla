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
        
        private List<Vector3> previousReachablePositions = new();
        private Vector3 lastTargetPosition;
        public override void ConfigureComponent(SimpleEntity assignedEntity)
        {
            base.ConfigureComponent(assignedEntity);
            Debug.Log("Configurado");
            lastTargetPosition = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
        }
        
        public virtual async Task CalculateReachablePositions(Grid3d levelGrid, Vector3 startWorldPos, int movementRange, int yieldFrequency = 100)
        {
            Debug.Log(lastTargetPosition +"; " +startWorldPos);
            
            if (Vector3.Distance(lastTargetPosition, startWorldPos) < 0.1f) return;
            
            lastTargetPosition = startWorldPos;
            await AStarPathFinder.InsertReachablePositionsAsyncBFS(previousReachablePositions, levelGrid, startWorldPos, movementRange, yieldFrequency);
        }
        
        public virtual async Task VisualizeMovement()
        {
            int moveRange = AssignedEntity.GetStats().MovementStat;
            
            await CalculateReachablePositions(AssignedEntity.Grid, EntityTransform.position, moveRange);
            
            AssignedEntity.LevelManager.LevelAsset.Grid.PaintTilesAtGridPositions(previousReachablePositions, movementColor);
        }
        public virtual void DoMove(Vector3 newPosition, bool undo)
        {
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
                var path = AStarPathFinder.FindPath(startGrid, newPosition, grid);
                if (path == null || path.Count == 0)
                {
                    Debug.LogWarning("No se encontró camino (FindPath devolvió null/empty).");
                    return;
                }
                
                if(AssignedEntity.TryGetLayeredEntity(out var layeredEntity)) layeredEntity.PlayAnimation("walk");
                
                AssignedEntity.StartCoroutine(MoveAlongPath(path));
            }
            
            AssignedEntity.GetFlags().RaiseFlag(EntityFlag.HasMoved);
        }

        public virtual async Task<bool> ValidateMove(Vector3 newPosition)
        {
            AssignedEntity.Grid.ResetPaint();
            
            int moveRange = AssignedEntity.GetStats().MovementStat;
            await CalculateReachablePositions(AssignedEntity.Grid, EntityTransform.position, moveRange);
            Vector3Int target = newPosition.CastToVectorInt();
            
            return previousReachablePositions.Contains(target);
        }
        private IEnumerator MoveAlongPath(List<Vector3> path)
        {
            var transform = EntityTransform;
            
            AssignedEntity.EntityController.PlaceEntityComponent.Remove();

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
            
            if(AssignedEntity.TryGetLayeredEntity(out var layeredEntity))
                layeredEntity.PlayAnimation("idle");
            
            AssignedEntity.EntityController.PlaceEntityComponent.Place();
        }
    }
}
