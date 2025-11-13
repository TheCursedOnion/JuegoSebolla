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
        enum MoveResult
        {
            Success,
            OnAir,
            OnWater,
            OnNotFullTile,
            OnFullTile,
            Impossible,
        }
        
        const string Model = "TileSelector Model";
        const string Controller = "TileSelector Controller";
        
        [SerializeField, ReadOnly] Vector3 gridPosition;
        
        [Inject] LevelManager levelManager;
        private LevelAsset levelAsset;
        private LevelEvents levelEvents;
        
        [Inject] CameraLocator cameraLocator;
        private GlobalCamera globalCamera;

        [SerializeField, BoxGroup(Model)] float yModelOffset = 0.05f;
        [SerializeField, BoxGroup(Model)] private GameObject tileModel;
        [SerializeField, BoxGroup(Controller)] TileSelectorController controller;

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
            
            controller.GetCurrentBehaviour().SoftSelect(SelectTile());
        }

        public void SelectEntity(SimpleEntity entity)
        {
            levelEvents.SelectEntity(entity);
        }
        public bool MovePosition(Vector3 moveDirection)
        {
            Vector3 newPosition = transform.position + moveDirection;
            MoveResult result = TrySetAtPosition(newPosition);
            switch (result)
            {
                case MoveResult.OnAir: return MovePosition(moveDirection - Vector3.up);
                case MoveResult.OnFullTile: return MovePosition(moveDirection + Vector3.up);
            }
            return result == MoveResult.Success || result == MoveResult.OnNotFullTile;
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
            return result == MoveResult.Success || result == MoveResult.OnNotFullTile;
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
        MoveResult TrySetAtPosition(Vector3 position)
        {
            Grid3d grid = levelAsset.Grid;
            
            if (!grid.TryWorldToGridPosition(position, out Vector3 gridPos)) return MoveResult.Impossible;

            Tile3d tile = grid.GetTileAtGridPosition(gridPos);
            bool isFull = tile.IsFullTile();
            bool isEmpty = tile.IsEmptyTile();
            bool isFluid = tile.IsFluidTile();
            
            if (isFull && !isEmpty)
                return MoveResult.OnFullTile;

            if (!isFull && !isEmpty)
            {
                SetPosition(gridPos, position, tile);
                return MoveResult.OnNotFullTile;
            }

            Vector3 belowPos = position - Vector3.up;
            if (!grid.TryWorldToGridPosition(belowPos, out Vector3 belowGridPos)) return MoveResult.Impossible;

            Tile3d belowTile = grid.GetTileAtGridPosition(belowGridPos);
            bool belowFull = belowTile.IsFullTile();
            bool belowEmpty = belowTile.IsEmptyTile();
            bool belowFluid = belowTile.IsFluidTile();

            if (belowEmpty || !belowFull) return MoveResult.OnAir;

            if (belowFluid)
            {
                Debug.Log("De momento permito que te puedas mover sobre fluidos");
                //return MoveResult.OnWater;
            }

            SetPosition(gridPos, position, tile);
            return MoveResult.Success;
        }
        
        void SetPosition(Vector3 gridPosition, Vector3 position, Tile3d onTile)
        {
            this.gridPosition = gridPosition;
            transform.position = position.CenterOnTile();
            
            float xRotation = !onTile.IsFullTile() && !onTile.IsEmptyTile() ? -45f : 0f;
            tileModel.transform.localPosition = onTile.GetDisplayOffset() + Vector3.up * yModelOffset;
            tileModel.transform.localEulerAngles = new Vector3(xRotation, 0, 0);

        }
        
        
    }
}
