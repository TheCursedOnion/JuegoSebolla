using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CursedOnion.Extensions;
using CursedOnion.Game.Cameras;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.Game.Systems.Level;
using CursedOnion.Helpers;
using CursedOnion.Locators;
using Reflex.Extensions;
using UnityEngine;

namespace CursedOnion.Game.Entity.Components
{
    [System.Serializable]
    public class MoveEntityComponent : EntityComponent
    {
        [SerializeField] protected Color movementColor = Color.blue;
        
        private List<Vector3> previousReachablePositions = new();
        private Vector3 lastTargetPosition;
        public override void ConfigureComponent(EntityComponentController controller)
        {
            base.ConfigureComponent(controller);
            lastTargetPosition = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
        }
        
        public virtual async Task CalculateReachablePositions(Grid3d levelGrid, Vector3 startWorldPos, int movementRange, int yieldFrequency = 100)
        {   
            if (Vector3.Distance(lastTargetPosition, startWorldPos) < 0.1f) return;
            
            lastTargetPosition = startWorldPos;
            await AStarPathFinder.InsertReachablePositionsAsyncBFS(previousReachablePositions, levelGrid, startWorldPos, movementRange, yieldFrequency);
        }
        
        public virtual async Task VisualizeMovement()
        {
            int moveRange = AssignedEntity.Stats.MovementStat;
            
            await CalculateReachablePositions(AssignedEntity.Grid, EntityTransform.position, moveRange);
            
            AssignedEntity.LevelManager.Grid.PaintTilesAtGridPositions(previousReachablePositions, movementColor);
        }
        public virtual void DoMove(Vector3 newPosition, bool undo)
        {
            var grid = AssignedEntity.Grid;
            var transform = EntityTransform;
            
            if (undo)
            {
                transform.position = newPosition;
                AssignedEntity.GetFlags().ResetFlag(UsedFlags);
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
            
            AssignedEntity.GetFlags().RaiseFlag(UsedFlags);
        }

        public virtual async Task<bool> ValidateMove(Vector3 newPosition)
        {
            AssignedEntity.Grid.ResetPaint();
            
            int moveRange = AssignedEntity.Stats.MovementStat;
            await CalculateReachablePositions(AssignedEntity.Grid, EntityTransform.position, moveRange);
            Vector3Int target = newPosition.CastToVectorInt();
            
            return previousReachablePositions.Contains(target);
        }
        private IEnumerator MoveAlongPath(List<Vector3> path)
        {
            var transform = EntityTransform;
            var camera = AssignedEntity.gameObject.scene.GetSceneContainer().Resolve<CameraLocator>().GlobalCamera;
            var placeComponent = AssignedController.GetEntityComponent<PlaceEntityComponent>();
            placeComponent.RemoveEntity();

            float speed = 5f;
            Vector3 lastPosition = transform.position;

            foreach (var position in path)
            {
                Vector3 direction = position - lastPosition;
                RotateEntity(camera, transform, direction);

                while (Vector3.Distance(transform.position, position) > 0.01f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, position, speed * Time.deltaTime);
                    yield return null;
                }

                transform.position = position;
                lastPosition = position;
                yield return null;
            }

            Unit unit = AssignedEntity as Unit;
            unit.UpdateStatusEffects();

            if (AssignedEntity.TryGetLayeredEntity(out var layeredEntity))
                layeredEntity.PlayAnimation("idle");
            
            placeComponent.PlaceEntity();
        }

        private void RotateEntity(GlobalCamera camera, Transform transform, Vector3 movementDirection)
        {
            float degrees = camera.GetCameraPanAngles();
            movementDirection = Quaternion.AngleAxis(-degrees, Vector3.up) * movementDirection;
            
            if (movementDirection.x > 0.01f)
            {
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x) * -1f;
                transform.localScale = scale;
            }
            else if (movementDirection.x < -0.01f)
            {
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x);
                transform.localScale = scale;
            }
        }
    }
}
