using CursedOnion.Game.Inputs;
using CursedOnion.Helpers;
using UnityEngine;
using UnityEngine.Serialization;

namespace CursedOnion.Behaviours
{
    public class CameraBehaviours : MonoBehaviour
    {
        [FormerlySerializedAs("cameraPlayable")] [SerializeField] Game.Inputs.CameraController CameraController;
        TransitionIndex transitionIndex = new TransitionIndex();
        
        bool isNone = false;
        bool isFreeMode = false;
        public void ChangeToNone() => transitionIndex.SetTransitionIndex(0);
        public bool CanChangeToNone() => transitionIndex.IsIndexEquals(0);
        public void EnterNone()
        {
            ResetControls();
            
            isNone = true;
        }
        
        public void ChangeToFreeMode() => transitionIndex.SetTransitionIndex(1);
        public bool CanChangeToFreeMode() => transitionIndex.IsIndexEquals(1);
        public void EnterFreeMode()
        {
            ResetControls();
            CameraController.EnableMove(true);
            CameraController.EnableRotate(true);
            
            isNone = false;
            isFreeMode = true;
        }
        
        public void ChangeToFixedMode() => transitionIndex.SetTransitionIndex(2);
        public bool CanChangeToFixedMode() => transitionIndex.IsIndexEquals(2);
        public void EnterFixedMode()
        {
            ResetControls();
            CameraController.EnableFollow(true);
            CameraController.EnableRotate(true);
            
            isNone = false;
            isFreeMode = false;
        }

        void ResetControls()
        {
            CameraController.EnableFollow(false);
            CameraController.EnableMove(false);
            CameraController.EnableRotate(false);
        }

        public void SwitchCameraModes()
        {
            if (!isNone)
            {
                if (isFreeMode)
                    ChangeToFixedMode();
                else
                    ChangeToFreeMode();
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
