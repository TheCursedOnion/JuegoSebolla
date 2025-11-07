using System;
using NaughtyAttributes;
using Reflex.Attributes;
using Reflex.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Linq;

using CursedOnion.Extensions;
using CursedOnion.Game.Cameras;
using CursedOnion.Game.Commands;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Events;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.Game.Systems.Level;
using CursedOnion.Locators;

namespace CursedOnion.Game.Objects
{
    public class SelectionData
    {
        public Vector3 GridPosition;
        public Tile3d Tile;
        
        public SelectionData(Vector3 gridPosition, Tile3d tile)
        {
            GridPosition = gridPosition;
            Tile = tile;
        }
    }
    public class TileSelector : MonoBehaviour
    {
        [SerializeField, ReadOnly] Vector3 gridPosition;
        
        [Inject] LevelManager levelManager;
        private LevelAsset levelAsset;
        private LevelEvents levelEvents;
        
        [Inject] CameraLocator cameraLocator;
        private GlobalCamera globalCamera;
        
        [SerializeField] TileSelectorController controller;

        private EntityCommandHandler entityCommandHandler;

        [SerializeReference, SubclassSelector, SerializeField] TileSelectorBehaviour[] behaviours;
        private T GetBehaviour<T>() where T : TileSelectorBehaviour
        {
            return behaviours.OfType<T>().FirstOrDefault();
        }
        
        public void Awake()
        {
            levelAsset = levelManager.LevelAsset;
            levelEvents = levelManager.LevelEvents;
            
            entityCommandHandler = new(gameObject.scene.GetSceneContainer());
            
            globalCamera = cameraLocator.GlobalCamera;

            foreach (var behaviour in behaviours)
            {
                behaviour.Initialize(this, entityCommandHandler);
            }
            
            controller.Initialize(this);
            UpdateBehaviour(LevelState.Finished, levelManager.CurrentLevelState);
        }

        private void OnEnable()
        {
            levelEvents.OnLevelStateChange += UpdateBehaviour;
        }

        private void OnDisable()
        {
            levelEvents.OnLevelStateChange += UpdateBehaviour;
        }

        void UpdateBehaviour(LevelState previousState, LevelState currentState)
        {
            switch (currentState)
            {
                default:
                case LevelState.InBattleEditor: controller.SetBehaviour(GetBehaviour<TileSelectorEditorBehaviour>());
                    break;
                
                case LevelState.InBattle: controller.SetBehaviour(GetBehaviour<TileSelectorBattleBehaviour>());
                    break;
            }
        }

        public void SelectEntity(SimpleEntity entity)
        {
            levelEvents.SelectEntity(entity);
        }
        public bool MovePosition(Vector3 moveDirection)
        {
            
            Vector3 newPosition = transform.position + moveDirection;
            int result = TrySetAtPosition(newPosition);
            switch (result)
            {
                case 1: MovePosition(moveDirection - Vector3.up); break;
                case 2: MovePosition(moveDirection + Vector3.up); break;
            }
            return result == 0; // 0 = success
        }
        public bool PlaceAtPointerPosition()
        {
            if (IsPointerOverUI()) return false;
            
            Ray ray = globalCamera.Camera.ScreenPointToRay(Input.mousePosition);
            int result = -1;
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 hitPoint = hit.point + hit.normal * 0.1f;
                result = TrySetAtPosition(hitPoint);
            }
            return result == 0;
        }
        bool IsPointerOverUI()
        {
            if (EventSystem.current == null) return false;
            
            if (EventSystem.current.IsPointerOverGameObject())
                return true;
            
            if (Touchscreen.current != null && Touchscreen.current.touches.Count > 0)
                return EventSystem.current.IsPointerOverGameObject(Touchscreen.current.touches[0].touchId.ReadValue());

            return false;
        }
        public SelectionData SelectTile()
        {
            Grid3d grid = levelAsset.Grid;
            Tile3d tile = grid.GetTileAtGridPosition(gridPosition);
            return new SelectionData(gridPosition, tile);
        }
        public 
        int TrySetAtPosition(Vector3 position)
        {
            Grid3d grid = levelAsset.Grid;
            
            if (!grid.TryWorldToGridPosition(position, out Vector3 gridPos)) return -1;
            
            Tile3d tile = grid.GetTileAtGridPosition(gridPos);
            if (!tile.IsEmptyTile())
            {
                return 2;
            }
            
            Vector3 belowPos = position - Vector3.up;
            if (!grid.TryWorldToGridPosition(belowPos, out Vector3 belowGridPos)) return -1;

            Tile3d belowTile = grid.GetTileAtGridPosition(belowGridPos);
            
            if (belowTile.IsEmptyTile()) return 1;
            
            gridPosition = gridPos;
            transform.position = position.CenterOnTile();
            return 0;
        }
        
    }
}
