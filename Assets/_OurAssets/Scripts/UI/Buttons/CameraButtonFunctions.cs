using CursedOnion.Game.Settings;
using CursedOnion.Helpers;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.UI
{
    public class CameraButtonFunctions : MonoBehaviour

    {
        [Inject] RuntimeSettings runtimeSettings;

        public void RotateCamera(DirectionFlag direction)
        {
            runtimeSettings.GlobalCamera.CameraController.RotateCamera(direction);
        }

        public void SwitchCameraModes()
        {
            runtimeSettings.GlobalCamera.SwitchCameraModes();
        }
    }
}