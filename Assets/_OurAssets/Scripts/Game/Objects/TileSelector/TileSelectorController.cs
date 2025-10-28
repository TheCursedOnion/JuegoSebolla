using System;
using CursedOnion.Extensions;
using CursedOnion.Game.Commands;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.Game.Events;
using CursedOnion.Game.Inputs;
using CursedOnion.Game.Systems.Level;
using CursedOnion.Locators;
using Reflex.Attributes;
using Reflex.Core;
using Reflex.Extensions;
using Reflex.Injectors;
using UnityEngine;

namespace CursedOnion.Game.Objects
{
    public class TileSelectorController : MonoBehaviour, IController
    {
        [Inject] public InputReaderCollection InputReaderCollection { get; set; }
        [Inject] private CameraLocator cameraLocator;
        [Inject] private LevelEvents levelEvents;
        
        TileSelector tileSelector;
        EntityCommandHandler entityCommandHandler;

        
        TileSelectorInputReader reader;
        void Awake()
        {
            reader = InputReaderCollection.GetReader<TileSelectorInputReader>();
            tileSelector = GetComponent<TileSelector>();

            entityCommandHandler = new EntityCommandHandler(gameObject.scene.GetSceneContainer());
            
            Enable();
        }

        #region Activar o Desactivar detección de Inputs
        private void OnEnable()
        {
            reader.PlaceSelector += PlaceSelector;
        }
        public void Enable()
        {
            Disable(); //Si no estaba suscrito, no pasa nada, y si lo estaba, evita que se suscriba más de una vez
            reader.MoveSelector += MoveSelector;
            reader.Select += Select;
        }
        private void OnDisable()
        {
            Disable();
            reader.PlaceSelector -= PlaceSelector;
        }
        public void Disable()
        {
            reader.MoveSelector -= MoveSelector;
            reader.Select -= Select;
        }
        #endregion
        
        void MoveSelector(Vector2 direction)
        {
            Vector3 direction3D = direction.normalized;
            direction3D = direction3D.SwizzleXZY();

            float rotateAngle = cameraLocator.GlobalCamera.GetCameraPanAngles();
            rotateAngle = Mathf.Round(rotateAngle % 90) == 0 ? rotateAngle : rotateAngle + 45;
            Quaternion rotation = Quaternion.AngleAxis(rotateAngle, Vector3.up);
            direction3D = rotation * direction3D;
            
            bool success = tileSelector.MovePosition(direction3D);
            if (success && !entityCommandHandler.HasPreparedCommand()) Select();
        }
        
        bool processPlacingNextFrame = false;
        void PlaceSelector()
        {
            processPlacingNextFrame = true;
        }

        void Select()
        { 
            var selectedTile = tileSelector.SelectTile();
            
            var entity = selectedTile.tile.GetContainedEntity();
            if (entityCommandHandler.HasPreparedCommand())
            {
                LaunchCommand(selectedTile.gridPosition, selectedTile.tile);
            }
            else
            {
                levelEvents.SelectEntity(entity);
            }
        }
        void LaunchCommand(Vector3 gridPosition, Tile3d tile)
        {
            EntityCommandParameters commandParameters = new(gridPosition, tile.GetContainedEntity());
            entityCommandHandler.LaunchCommand(commandParameters);
        }
        
        void Update()
        {
            if (processPlacingNextFrame)
            {
                processPlacingNextFrame = false;
                bool success = tileSelector.PlaceAtPointerPosition();
                if (success) Select();
            }
        }
    }
}