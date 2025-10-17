using CursedOnion.Extensions;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.Game.Cameras;
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
        [Inject] public InputReaderCollection InputReaderCollection { get; set; }
        [Inject] private CameraLocator cameraLocator;
        TileSelectorInputReader reader;
        void Awake()
        {
            reader = InputReaderCollection.GetReader<TileSelectorInputReader>();
            tileSelector = GetComponent<TileSelector>();
        }

        private void OnEnable()
        {
            reader.Select += PlaceSelector;
        }
        public void Enable()
        {
            reader.MoveSelector += MoveSelector;
            reader.Inspect += Inspect;
        }

        private void OnDisable()
        {
            Disable();
            reader.Select -= PlaceSelector;
        }
        public void Disable()
        {
            reader.MoveSelector -= MoveSelector;
            reader.Inspect -= Inspect;
        }

        void MoveSelector(Vector2 direction)
        {
            Vector3 direction3D = direction.normalized;
            direction3D = direction3D.SwizzleXZY();

            float rotateAngle = cameraLocator.GlobalCamera.GetCameraPanAngles();
            rotateAngle = Mathf.Round(rotateAngle % 90) == 0 ? rotateAngle : rotateAngle + 45;
            Quaternion rotation = Quaternion.AngleAxis(rotateAngle, Vector3.up);
            direction3D = rotation * direction3D;
            
            tileSelector.MovePosition(direction3D);
        }

        void PlaceSelector()
        {
            tileSelector.AttemptToPlaceAtPointerPosition();
        }

        void Inspect()
        {
            tileSelector.InspectSelectedElement();
        }
    }
}