using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CursedOnion.Extensions;
using CursedOnion.Game.Systems.Grid;
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
        public override EntityComponent ConfigureComponent(EntityComponentController controller)
        {
            base.ConfigureComponent(controller);
            lastTargetPosition = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
            return this;
        }
        
        public virtual async Task CalculateReachablePositions(Grid3d levelGrid, Vector3 startWorldPos, int movementRange, int yieldFrequency = 100)
        {   
            if (Vector3.Distance(lastTargetPosition, startWorldPos) < 0.1f) return;
            
            lastTargetPosition = startWorldPos;
            await AStarPathFinder.InsertReachableGridPositionsAsyncBFS(previousReachablePositions, levelGrid, AssignedEntity.GetSide(), startWorldPos, movementRange, yieldFrequency);
        }
        
        public virtual async Task VisualizeMovement()
        {
            int moveRange = AssignedEntity.Stats.MovementStat + AssignedEntity.StatusHandler.AdditionalMovement;

            await CalculateReachablePositions(AssignedEntity.Grid, EntityTransform.position, moveRange);
            
            AssignedEntity.LevelManager.Grid.PaintTilesAtGridPositions(previousReachablePositions, movementColor);
        }
        public virtual void DoMove(Vector3 newPosition, bool undo)
        {
            var grid = AssignedEntity.Grid;
            var transform = EntityTransform;
            
            if (undo)
            {
                Debug.Log($"{AssignedEntity.name} ha hecho undo a {newPosition}. unidad?:{AssignedEntity is Unit} ");
                
                AssignedEntity.EntityController.PlaceComponent.RemoveEntity();
                transform.position = newPosition;
                AssignedEntity.EntityController.PlaceComponent.PlaceEntity();
                
                (AssignedEntity as Unit)?.FocusOnUnit();
                
                AssignedEntity.ActionHandler.ResetFlag(UsedFlags);
            }
            else
            {
                if (!AssignedEntity.Grid.TryWorldToGridPosition(transform.position, out Vector3 startGrid))
                {
                    return;
                }
                
                grid.ResetPaint();
                var path = AStarPathFinder.FindPath(startGrid, newPosition, grid, AssignedEntity.GetSide());
                if (path == null || path.Count == 0)
                {
                    Debug.LogWarning("No se encontró camino (FindPath devolvió null/empty).");
                    AssignedEntity.LevelEvents.InvokePathNotFound();
                    AssignedEntity.ActionHandler.RaiseFlag(UsedFlags);
                    return;
                }
                
                if(AssignedEntity.TryGetLayeredEntity(out var layeredEntity)) layeredEntity.PlayAnimation("walk");
                
                AssignedEntity.StartCoroutine(MoveAlongPath(path));
                AssignedEntity.ActionHandler.RaiseFlag(UsedFlags);
            }
        }

        public virtual async Task<bool> ValidateMove(Vector3 newPosition)
        {
            AssignedEntity.Grid.ResetPaint();
            
            int moveRange = AssignedEntity.Stats.MovementStat + AssignedEntity.StatusHandler.AdditionalMovement;
            await CalculateReachablePositions(AssignedEntity.Grid, EntityTransform.position, moveRange);
            Vector3Int target = newPosition.CastToVectorInt();
            
            bool onTile = AssignedEntity.Grid.TryGetTileAtGridPosition(newPosition, out Tile3d tile);
            return previousReachablePositions.Contains(target) && onTile && !tile.IsBlocked();
        }
        private IEnumerator MoveAlongPath(List<Vector3> path)
        {
            var transform = EntityTransform;
            var camera = AssignedEntity.gameObject.scene.GetSceneContainer().Resolve<RuntimeVariableLocator>().GlobalCamera;
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

            if (AssignedEntity.TryGetLayeredEntity(out var layeredEntity))
                layeredEntity.PlayAnimation("idle");
            
            placeComponent.PlaceEntity();
        }
    }
}
