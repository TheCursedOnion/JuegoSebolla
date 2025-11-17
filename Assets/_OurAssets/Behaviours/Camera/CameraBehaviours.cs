using CursedOnion.Game.Cameras;
using CursedOnion.Game.Events;
using CursedOnion.Game.Inputs;
using CursedOnion.Helpers;
using UnityEngine;

namespace CursedOnion.Behaviours
{
    public enum CameraMode
    {
        None = 0,
        FreeMode = 1,
        FixedMode = 2,
    }
    public class CameraBehaviours : MonoBehaviour
    {
        CameraEvents cameraEvents;
        CameraController cameraController;
        
        TransitionIndex transitionIndex = new TransitionIndex();
        
        CameraMode currentMode = CameraMode.None;
        public void Initialize(GlobalCamera globalCamera)
        {
            this.cameraController = globalCamera.CameraController;
            this.cameraEvents = globalCamera.CameraEvents;
        }
        public void ChangeToNone() => transitionIndex.SetTransitionIndex((int)CameraMode.None);
        public bool CanChangeToNone() => transitionIndex.IsIndexEquals((int)CameraMode.None);
        public void EnterNone()
        {
            cameraController.DisableAll();

            currentMode = CameraMode.None;
            cameraEvents.OnCameraModeModified(currentMode);
        }
        
        public void ChangeToFreeMode() => transitionIndex.SetTransitionIndex((int)CameraMode.FreeMode);
        public bool CanChangeToFreeMode() => transitionIndex.IsIndexEquals((int)CameraMode.FreeMode);
        public void EnterFreeMode()
        {
            cameraController.DisableAll();
            
            cameraController.EnableFreeMode();
            cameraController.EnableRotate(true);
            
            currentMode = CameraMode.FreeMode;
            cameraEvents.OnCameraModeModified(currentMode);
        }

        public void ChangeToFixedMode() => transitionIndex.SetTransitionIndex((int)CameraMode.FixedMode);
        public bool CanChangeToFixedMode() => transitionIndex.IsIndexEquals((int)CameraMode.FixedMode);
        public void EnterFixedMode()
        {
            cameraController.DisableAll();
            
            cameraController.EnableFixedMode();
            cameraController.EnableRotate(true);
            
            currentMode = CameraMode.FixedMode;
            cameraEvents.OnCameraModeModified(currentMode);
        }

        public void SwitchCameraModes()
        {
            Debug.Log($"Switching camera modes from {currentMode} to {(currentMode == CameraMode.FreeMode ? CameraMode.FixedMode : CameraMode.FreeMode)}");
            switch (currentMode)
            {
                case CameraMode.FreeMode: EnterFixedMode(); break;
                case CameraMode.FixedMode: EnterFreeMode(); break;
            }
        }
    }
}
