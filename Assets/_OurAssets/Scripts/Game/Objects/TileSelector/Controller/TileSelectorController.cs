using System;
using CursedOnion.Extensions;
using CursedOnion.Game.Inputs;
using CursedOnion.Game.Logic.Services.Pause;
using CursedOnion.Locators;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Objects
{
    public class TileSelectorController : MonoBehaviour, IController, IPausable
    {
        [Inject] public InputReaderCollection InputReaderCollection { get; set; }
        [Inject] private RuntimeVariableLocator runtimeVariableLocator;

        TileSelector tileSelector;
        TileSelectorBehaviour currentBehaviour;
        public TileSelectorBehaviour GetCurrentBehaviour() => currentBehaviour;
        public void SetBehaviour(TileSelectorBehaviour behaviour)
        {
            currentBehaviour = behaviour;
        }
        
        TileSelectorInputReader reader;
        public void Initialize(TileSelector tileSelector)
        {
            this.tileSelector = tileSelector;
            reader ??= InputReaderCollection.GetReader<TileSelectorInputReader>();
            
            Enable();
        }

        #region Activar o Desactivar detección de Inputs
        private void OnEnable()
        {
            reader ??= InputReaderCollection.GetReader<TileSelectorInputReader>();
            reader.PlaceSelector += PlaceSelector;
            Enable();
        }
        public void Enable()
        {
            Disable();
            reader.Enable();
            reader.PlaceSelector += PlaceSelector;
            reader.MoveSelector += MoveSelector;
            reader.Select += HardSelect;
        }
        private void OnDisable()
        {
            Disable();
        }
        public void Disable()
        {
            reader.Disable();
            reader.PlaceSelector -= PlaceSelector;
            reader.MoveSelector -= MoveSelector;
            reader.Select -= HardSelect;
        }
        
        public void Pause() => reader.Disable();
        public void Unpause() => reader.Enable();
        #endregion
        
        void MoveSelector(Vector2 direction)
        {
            Vector3 direction3D = direction.normalized;
            direction3D = direction3D.SwizzleXZY();

            float rotateAngle = runtimeVariableLocator.GlobalCamera.GetCameraPanAngles();
            rotateAngle = Mathf.Round(rotateAngle % 90) == 0 ? rotateAngle : rotateAngle + 45;
            Quaternion rotation = Quaternion.AngleAxis(rotateAngle, Vector3.up);
            direction3D = rotation * direction3D;
            
            bool success = tileSelector.MovePosition(direction3D);
            if (success) SoftSelect();
        }
        
        bool processPlacingNextFrame = false;
        void PlaceSelector()
        {
            processPlacingNextFrame = true;
        }

        void HardSelect()
        { 
            var data = tileSelector.SelectTile();
            currentBehaviour.HardSelect(data);
        }
        void SoftSelect()
        {
            var data = tileSelector.SelectTile();
            currentBehaviour.SoftSelect(data);
        }
        void Update()
        {
            if (processPlacingNextFrame)
            {
                processPlacingNextFrame = false;
                bool success = tileSelector.PlaceAtPointerPosition();
                if (success) HardSelect();
            }
        }
    }
}