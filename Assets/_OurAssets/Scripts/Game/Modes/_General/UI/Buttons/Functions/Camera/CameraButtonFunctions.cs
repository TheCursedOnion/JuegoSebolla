using System;
using CursedOnion.Extensions;
using CursedOnion.Game.Cameras;
using CursedOnion.Game.Inputs.Camera;
using CursedOnion.Game.Settings;
using CursedOnion.Helpers;
using CursedOnion.Locators;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace CursedOnion.Game.General.UI.Buttons.Functions
{
    public class CameraButtonFunctions : MonoBehaviour
    {
        [Inject] RuntimeVariableLocator runtimeVariableLocator;
        private GlobalCamera globalCamera;
        
        [SerializeField] private Image switchCameraModeImage;
        [SerializeField] private Image rotateLeftImage;
        [SerializeField] private Image rotateRightImage;
        
        [SerializeField] private Color freeModeColor;
        [SerializeField] private Color fixedModeColor;
        [SerializeField] private Color disabledModeColor;
        
        private void Awake()
        {
            gameObject.InjectDependencies();
            globalCamera = runtimeVariableLocator.GlobalCamera;
        }
        private void OnEnable()
        {
            globalCamera.CameraEvents.OnModifyCameraMode += UpdateCameraModeImage;
            UpdateCameraModeImage(globalCamera.CameraController.GetCurrentMode());
        }
        private void OnDisable()
        {
            globalCamera.CameraEvents.OnModifyCameraMode -= UpdateCameraModeImage;
        }

        public void RotateCamera(DirectionFlag direction)
        {
            globalCamera.CameraController.RotateCamera(direction);
        }

        public void SwitchCameraModes()
        {
            globalCamera.SwitchCameraModes();
        }

        void UpdateCameraModeImage(CameraControlFlag flag)
        {
            Color color = flag switch
            {
                CameraControlFlag.FreeMode => freeModeColor,
                CameraControlFlag.FixedMode => fixedModeColor,
                _ => disabledModeColor
            };
            
            switchCameraModeImage.color = color;
            rotateLeftImage.color = color;
            rotateRightImage.color = color;

        }
    }
}