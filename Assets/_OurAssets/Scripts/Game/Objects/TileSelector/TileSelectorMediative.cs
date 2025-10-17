using CursedOnion.Behaviours;
using CursedOnion.Game.Logic;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Objects
{
    public class TileSelectorMediative : MonoBehaviour, IMediative
    {
        [Inject] public MediatorEvents MediatorEvents { get; set; }
        
        TileSelectorController tileSelectorController;
        void Awake()
        {
            tileSelectorController = GetComponent<TileSelectorController>();
        }
        
        void OnEnable()
        {
            MediatorEvents.OnModifyCameraMode += OnCameraModification;
        }

        void OnDisable()
        {
            MediatorEvents.OnModifyCameraMode -= OnCameraModification;
        }

        void OnCameraModification(CameraMode cameraMode)
        {
            switch (cameraMode)
            {
                case CameraMode.FreeMode: tileSelectorController.Disable(); break;
                case CameraMode.FixedMode: tileSelectorController.Enable(); break;
            }
        }
    }
}
