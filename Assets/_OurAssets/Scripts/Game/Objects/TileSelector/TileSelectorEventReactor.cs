using CursedOnion.Game.Events;
using CursedOnion.Game.Inputs.Camera;
using CursedOnion.Locators;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Objects
{
    public class TileSelectorEventReactor : MonoBehaviour
    {
        [Inject] RuntimeVariableLocator runtimeVariableLocator;
        CameraEvents cameraEvents;

        TileSelectorController tileSelectorController;
        void Awake()
        {
            tileSelectorController = GetComponent<TileSelectorController>();
            cameraEvents = runtimeVariableLocator.GlobalCamera.CameraEvents;
        }
        
        void OnEnable()
        {
            cameraEvents.OnModifyCameraMode += OnCameraModification;
            //levelAsset.LevelEvents.OnCommandCalled += ;
        }

        void OnDisable()
        {
            cameraEvents.OnModifyCameraMode -= OnCameraModification;
        }

        void OnCameraModification(CameraControlFlag cameraMode)
        {
            Debug.Log($"Camera mode changed to {cameraMode}");
            switch (cameraMode)
            {
                case CameraControlFlag.FreeMode: tileSelectorController.Disable(); break;
                case CameraControlFlag.FixedMode: tileSelectorController.Enable(); break;
            }
        }
    }
}
