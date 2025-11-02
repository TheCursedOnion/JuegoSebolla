using CursedOnion.Behaviours;
using CursedOnion.Game.Events;
using CursedOnion.Game.Systems.Level;
using CursedOnion.Locators;
using CursedOnion.ScriptableObjects;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Objects
{
    public class TileSelectorEventReactor : MonoBehaviour
    {
        [Inject] LevelAsset levelAsset;
        [Inject] CameraLocator cameraLocator;
        CameraEvents cameraEvents;

        TileSelectorController tileSelectorController;
        void Awake()
        {
            tileSelectorController = GetComponent<TileSelectorController>();
            cameraEvents = cameraLocator.GlobalCamera.CameraEvents;
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

        void OnCameraModification(CameraMode cameraMode)
        {
            Debug.Log($"Camera mode changed to {cameraMode}");
            switch (cameraMode)
            {
                case CameraMode.FreeMode: tileSelectorController.Disable(); break;
                case CameraMode.FixedMode: tileSelectorController.Enable(); break;
            }
        }
    }
}
