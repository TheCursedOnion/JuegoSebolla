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
            cameraEvents.OnCameraFollow += OnCameraModification;
            //levelAsset.LevelEvents.OnCommandCalled += ;
        }

        void OnDisable()
        {
            cameraEvents.OnCameraFollow -= OnCameraModification;
        }

        void OnCameraModification(Transform currentFollow)
        {
            bool enable = currentFollow == this.transform;
            if(enable)
                tileSelectorController.Enable();
            else
                tileSelectorController.Disable();
        }
    }
}
