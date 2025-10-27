using System;
using CursedOnion.Extensions;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.Game.Cameras;
using CursedOnion.Game.Commands;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Events;
using CursedOnion.Game.Handlers;
using CursedOnion.Game.Inputs;
using CursedOnion.Game.Logic;
using CursedOnion.Game.Settings;
using CursedOnion.Locators;
using CursedOnion.ScriptableObjects;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Objects
{
    public class TileSelectorController : MonoBehaviour, IController
    {
        TileSelector tileSelector;
        
        [Inject] LevelManager levelManager;
        EntityCommandHandler entityCommandHandler;
        [Inject] public InputReaderCollection InputReaderCollection { get; set; }
        [Inject] private CameraLocator cameraLocator;
        
        TileSelectorInputReader reader;
        void Awake()
        {
            reader = InputReaderCollection.GetReader<TileSelectorInputReader>();
            tileSelector = GetComponent<TileSelector>();
            
            entityCommandHandler = levelManager.CommandHandler;
            
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
        
        #region Command Sender
        void SendCommand(Vector3 gridPosition, Tile3d tile)
        {
            EntityCommandParameters commandParameters = new(gridPosition, tile.GetContainedEntity());
            entityCommandHandler.LaunchPreparedCommandWithParameters(commandParameters);
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
                SendCommand(selectedTile.gridPosition, selectedTile.tile);
            }
            else
            {
                entityCommandHandler.TrySelectEntity(entity);
            }
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