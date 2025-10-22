using System;
using CursedOnion.Extensions;
using CursedOnion.Game.Cameras;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Events;
using CursedOnion.Game.Settings;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.Locators;
using CursedOnion.ScriptableObjects;
using NaughtyAttributes;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace CursedOnion.Game.Objects
{
    public class TileSelector : MonoBehaviour
    {
        [SerializeField] Vector3 CameraOffset;
        [SerializeField, ReadOnly] Vector3 gridPosition;
        
        [Inject] LevelAsset levelAsset;
        
        [Inject] CameraLocator cameraLocator;
        private GlobalCamera globalCamera;
        
        [Inject] LevelEvents levelEvents;

        public void Awake()
        {
            globalCamera = cameraLocator.GlobalCamera;
            globalCamera.CinemachineContainer.SetTarget(transform, CameraOffset);
            globalCamera.CinemachineContainer.SetTiltCenterAndValue(35);
            globalCamera.CameraBehaviours.ChangeToFixedMode();
        }

        public void MovePosition(Vector3 moveDirection)
        {
            Vector3 newPosition = transform.position + moveDirection;
            int result = TrySetAtPosition(newPosition);

            switch (result)
            {
                case 1: MovePosition(moveDirection - Vector3.up); break;
                case 2: MovePosition(moveDirection + Vector3.up); break;
            }
        }
        
        bool processPlacingNextFrame = false;
        public void AttemptToPlaceAtPointerPosition()
        {
            processPlacingNextFrame = true;
        }

        void PlaceAtPointerPosition()
        {
            if (IsPointerOverUI()) return;
            
            Ray ray = globalCamera.Camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 hitPoint = hit.point + hit.normal * 0.1f;
                int result = TrySetAtPosition(hitPoint);
                
                if(result == 0) InspectSelectedElement();
            }
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
        public void InspectSelectedElement()
        {
            Grid3d grid = levelAsset.Grid;
            Tile3d tile = grid.GetTileAtGridPosition(gridPosition);

            Entity.SimpleEntity entity = tile.GetContainedEntity();
            levelEvents.OnEntityInspection(entity);
            
        }
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
        
        void Update()
        {
            if (processPlacingNextFrame)
            {
                processPlacingNextFrame = false;
                PlaceAtPointerPosition();
            }
        }
    }
}
