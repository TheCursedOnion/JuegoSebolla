using CursedOnion.Game.Settings;
using CursedOnion.Helpers;
using CursedOnion.Locators;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.General.UI.Buttons.Functions
{
    public class CameraButtonFunctions : MonoBehaviour

    {
        [Inject] RuntimeVariableLocator runtimeVariableLocator;

        public void RotateCamera(DirectionFlag direction)
        {
            runtimeVariableLocator.GlobalCamera.CameraController.RotateCamera(direction);
        }

        public void SwitchCameraModes()
        {
            runtimeVariableLocator.GlobalCamera.SwitchCameraModes();
        }
    }
}