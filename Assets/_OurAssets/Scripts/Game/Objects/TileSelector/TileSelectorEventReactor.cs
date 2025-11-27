using CursedOnion.Game.Events;
using CursedOnion.Game.Inputs.Camera;
using CursedOnion.Game.Systems.Level;
using CursedOnion.Locators;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Objects
{
    public class TileSelectorEventReactor : MonoBehaviour
    {
        [Inject] RuntimeVariableLocator runtimeVariableLocator;
        [Inject] LevelEvents levelEvents;
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
            levelEvents.OnIntroCalled += DisableReader;
            levelEvents.OnIntroFinished += EnableReader;
            //levelAsset.LevelEvents.OnCommandCalled += ;
        }

        void OnDisable()
        {
            cameraEvents.OnCameraFollow -= OnCameraModification;
            levelEvents.OnIntroCalled -= DisableReader;
            levelEvents.OnIntroFinished -= EnableReader;
        }

        void DisableReader()
        {
            tileSelectorController.EnableReader(false);
        }
        void EnableReader()
        {
            tileSelectorController.EnableReader(true);
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
