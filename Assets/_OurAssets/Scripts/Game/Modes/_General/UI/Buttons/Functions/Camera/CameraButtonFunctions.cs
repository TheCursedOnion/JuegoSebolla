using CursedOnion.Game.Settings;
using CursedOnion.Helpers;
using CursedOnion.Locators;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.General.UI.Buttons.Functions
{
    public class CameraButtonFunctions : MonoBehaviour

    {
        [Inject] CameraLocator cameraLocator;

        public void RotateCamera(DirectionFlag direction)
        {
            cameraLocator.GlobalCamera.CameraController.RotateCamera(direction);
        }

        public void SwitchCameraModes()
        {
            cameraLocator.GlobalCamera.SwitchCameraModes();
        }
    }
}