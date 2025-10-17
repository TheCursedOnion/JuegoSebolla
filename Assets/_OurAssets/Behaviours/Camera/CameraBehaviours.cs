using CursedOnion.Game.Inputs;
using CursedOnion.Game.Logic;
using CursedOnion.Helpers;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.Serialization;

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
        [Inject] MediatorEvents mediatorEvents;
        
        CameraController cameraController;
        TransitionIndex transitionIndex = new TransitionIndex();
        
        CameraMode currentMode = CameraMode.None;
        public void Initialize(CameraController cameraController)
        {
            this.cameraController = cameraController;
        }
        public void ChangeToNone() => transitionIndex.SetTransitionIndex((int)CameraMode.None);
        public bool CanChangeToNone() => transitionIndex.IsIndexEquals((int)CameraMode.None);
        public void EnterNone()
        {
            cameraController.DisableAll();

            currentMode = CameraMode.None;
            mediatorEvents.OnCameraModeModified(currentMode);
        }
        
        public void ChangeToFreeMode() => transitionIndex.SetTransitionIndex((int)CameraMode.FreeMode);
        public bool CanChangeToFreeMode() => transitionIndex.IsIndexEquals((int)CameraMode.FreeMode) || transitionIndex.IsIndexEquals(4);
        public void EnterFreeMode()
        {
            cameraController.DisableAll();
            
            cameraController.EnableMove(true);
            cameraController.EnableRotate(true);
            
            currentMode = CameraMode.FreeMode;
            mediatorEvents.OnCameraModeModified(currentMode);
        }

        public void ChangeToFixedMode() => transitionIndex.SetTransitionIndex((int)CameraMode.FixedMode);
        public bool CanChangeToFixedMode() => transitionIndex.IsIndexEquals((int)CameraMode.FixedMode) || transitionIndex.IsIndexEquals(4);
        public void EnterFixedMode()
        {
            cameraController.DisableAll();
            
            cameraController.EnableFollow(true);
            cameraController.EnableRotate(true);
            
            currentMode = CameraMode.FixedMode;
            mediatorEvents.OnCameraModeModified(currentMode);
        }

        public void SwitchCameraModes()
        {
            if (currentMode != CameraMode.None)
            {
                transitionIndex.SetTransitionIndex(4);
            }
        }
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Alpha0)) ChangeToNone();
            else if(Input.GetKeyDown(KeyCode.Alpha1)) ChangeToFreeMode();
            else if(Input.GetKeyDown(KeyCode.Alpha2)) ChangeToFixedMode();
            
            //Debug.LogWarning(transitionIndex.transitionIndex);
        }
    }
}
