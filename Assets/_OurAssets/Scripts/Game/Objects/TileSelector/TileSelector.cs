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
using Reflex.Injectors;

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
        enum MoveResult
        {
            Success,
            TryDown,
            TryUp,
            Impossible,
        }
        
        const string Model = "TileSelector Model";
        const string Controller = "TileSelector Controller";
        
        [SerializeField, ReadOnly] Vector3 gridPosition;
        
        [Inject] LevelManager levelManager;
        [Inject] Grid3d grid;
        [Inject] LevelEvents levelEvents;
        [Inject] CameraLocator cameraLocator;
        
        private GlobalCamera globalCamera;

        [SerializeField, BoxGroup(Model)] float yModelOffset = 0.05f;
        [SerializeField, BoxGroup(Model)] private GameObject tileModel;
        [SerializeField, BoxGroup(Controller)] TileSelectorController controller;

        private EntityCommandHandler entityCommandHandler;
        EntityCommandHandler EntityCommandHandler => entityCommandHandler;

        [SerializeReference, SubclassSelector, SerializeField] TileSelectorBehaviour[] behaviours;
        private T GetBehaviour<T>() where T : TileSelectorBehaviour
        {
            return behaviours.OfType<T>().FirstOrDefault();
        }
        
        public void Awake()
        {
            var container = gameObject.scene.GetSceneContainer();
            AttributeInjector.Inject(this, container);
            
            entityCommandHandler = new(container);
            
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
            levelEvents.OnTurnFocus += FocusOnEntity;
            levelEvents.OnLevelStateChange += UpdateBehaviour;
            levelEvents.OnStatDataSelected += ProcessStatDataUpdate;
        }

        private void OnDisable()
        {
            levelEvents.OnTurnFocus -= FocusOnEntity;
            levelEvents.OnLevelStateChange -= UpdateBehaviour;
            levelEvents.OnStatDataSelected -= ProcessStatDataUpdate;
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
            
            entityCommandHandler.ClearCommandStack();
            controller.GetCurrentBehaviour().SoftSelect(SelectTile());
        }

        public void ProcessStatDataUpdate(StatData statData)
        {
            if (!statData) InvokeEntitySelection(SelectTile().Tile.GetContainedEntity());
        }
        public void InvokeEntitySelection(SimpleEntity entity)
        {
            levelEvents.SelectEntity(entity);
        }
        void FocusOnEntity(SimpleEntity entity)
        {
            TrySetAtPosition(entity.transform.position);
        }
        public bool MovePosition(Vector3 moveDirection)
        {
            Vector3 newPosition = transform.position.CenterOnTile() + moveDirection;
            MoveResult result = TrySetAtPosition(newPosition);
            switch (result)
            {
                case MoveResult.TryUp: return MovePosition(moveDirection + Vector3.up);
                case MoveResult.TryDown: return MovePosition(moveDirection - Vector3.up);
            }
            return result == MoveResult.Success;
        }
        public bool PlaceAtPointerPosition()
        {
            if (IsPointerOverUI()) return false;
            
            Ray ray = globalCamera.Camera.ScreenPointToRay(Input.mousePosition);
            MoveResult result = MoveResult.Impossible;
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 hitPoint = hit.point + hit.normal * 0.1f;
                result = TrySetAtPosition(hitPoint);
            }
            return result == MoveResult.Success;
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
            Tile3d tile = grid.GetTileAtGridPosition(gridPosition);
            return new SelectionData(gridPosition, tile);
        }
        MoveResult TrySetAtPosition(Vector3 position)
        {
            if (!grid.TryWorldToGridPosition(position, out Vector3 gridPos)) return MoveResult.Impossible;
            Vector3 abovePos = position + Vector3.up;
            Vector3 belowPos = position - Vector3.up;
            
            
            Tile3d tile = grid.GetTileAtGridPosition(gridPos);
            bool isFull = tile.IsFullTile();
            bool isEmpty = tile.IsEmptyTile();
            bool isFluid = tile.IsFluidTile();
            bool isStair = tile.IsStairTile();
            
            if (isFull) return MoveResult.TryUp;
            
            if (isStair)
            {
                SetPosition(gridPos, position, tile);
                return MoveResult.Success;
            }

            if (isEmpty)
            {
                if (grid.TryWorldToGridPosition(abovePos, out Vector3 aboveGridPos))
                {
                    Tile3d aboveTile = grid.GetTileAtGridPosition(aboveGridPos);
                    if (aboveTile.IsStairTile()) return MoveResult.TryUp;
                }

                if (!grid.TryWorldToGridPosition(belowPos, out Vector3 belowGridPos)) return MoveResult.Impossible;
                Tile3d belowTile = grid.GetTileAtGridPosition(belowGridPos);
                
                bool belowFull = belowTile.IsFullTile();
                bool belowEmpty = belowTile.IsEmptyTile();
                bool belowFluid = belowTile.IsFluidTile();
                bool belowStair = belowTile.IsStairTile();

                if (belowEmpty || belowStair)
                {
                    return MoveResult.TryDown;
                }
                if (belowFluid || belowFull)
                {
                    SetPosition(gridPos, position, tile);
                    return MoveResult.Success;
                }
            }

            if (isFluid) return MoveResult.TryUp;
            
            return MoveResult.Impossible;
        }
        
        void SetPosition(Vector3 gridPosition, Vector3 worldPosition, Tile3d onTile)
        {
            this.gridPosition = gridPosition;
            transform.position = worldPosition.CenterOnTile();
            
            float xRotation = onTile.IsStairTile() ? -45f : 0f;
            float yRotation = onTile.GetYRotation();
            float zScale = onTile.IsStairTile() ? 1.414f : 1.1f;
            float yOffset = onTile.IsStairTile() ? 0f : yModelOffset;
            
            tileModel.SetActive(true);
            tileModel.transform.localPosition = onTile.GetDisplayOffset() + Vector3.up * yOffset;
            tileModel.transform.localEulerAngles = new Vector3(xRotation, yRotation, 0);
            tileModel.transform.localScale = new Vector3(1.1f, 1, zScale);

        }
        
        
    }
}
