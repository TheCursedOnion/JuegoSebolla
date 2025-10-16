using CursedOnion.Extensions;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.Game.Cameras;
using CursedOnion.Game.Inputs;
using CursedOnion.Game.Settings;
using CursedOnion.ScriptableObjects;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Objects
{
    public class TileSelectorController : MonoBehaviour, IController
    {
        TileSelector tileSelector;
        [Inject] public InputReaderCollection InputReaderCollection { get; set; }
        [Inject] private RuntimeSettings runtimeSettings;

        void Awake()
        {
            tileSelector = GetComponent<TileSelector>();
        }

        private void OnEnable() => Enable();
        public void Enable()
        {
            runtimeSettings.GlobalCamera.CinemachineContainer.CinemachineCamera.Follow = this.gameObject.transform;
            
            TileSelectorInputReader reader = InputReaderCollection.GetReader<TileSelectorInputReader>();
            reader.MoveSelector += MoveSelector;
            reader.Select += PlaceSelector;
            reader.Inspect += Inspect;
        }
        
        private void OnDisable() => Disable();
        public void Disable()
        {
            TileSelectorInputReader reader = InputReaderCollection.GetReader<TileSelectorInputReader>();
            reader.MoveSelector -= MoveSelector;
            reader.Select -= PlaceSelector;
        }

        void MoveSelector(Vector2 direction)
        {
            Vector3 direction3D = direction.normalized;
            direction3D = direction3D.SwizzleXZY();

            float rotateAngle = runtimeSettings.GlobalCamera.GetCameraPanAngles();
            rotateAngle = Mathf.Round(rotateAngle % 90) == 0 ? rotateAngle : rotateAngle + 45;
            Quaternion rotation = Quaternion.AngleAxis(rotateAngle, Vector3.up);
            direction3D = rotation * direction3D;
            
            tileSelector.MovePosition(direction3D);
        }

        void PlaceSelector()
        {
            tileSelector.PlaceAtMousePosition();
        }

        void Inspect()
        {
            tileSelector.InspectSelectedElement();
        }
    }
}