using System;
using CursedOnion.Game.Cameras;
using CursedOnion.Game.Logic.Services.Pause;
using CursedOnion.Helpers;
using CursedOnion.Locators;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Inputs.Camera
{
    [Flags]
    public enum CameraControlFlag
    {
        None = 0,
        Move = 1 << 0,
        Rotate = 1 << 1,
        Drag = 1 << 2,
        Zoom = 1 << 3,
        
        FreeMode = Move | Rotate | Drag | Zoom,
        FixedMode = Rotate | Zoom,
        Disabled = None
    }
    [RequireComponent(typeof(GlobalCamera))]
    public class CameraController : MonoBehaviour, IPausable, IDisposable
    {
        [Inject] public InputReaderCollection InputReaderCollection { get; set; }
        [Inject] RuntimeVariableLocator runtimeVariableLocator;
        
        [SerializeField] private CameraControlFlag cameraControlFlag = CameraControlFlag.Disabled;
        
        [SerializeField] private MoveComponent moveComponent = new();
        [SerializeField] private RotateComponent rotateComponent = new();
        [SerializeField] private DragComponent dragComponent = new();
        [SerializeField] private ZoomComponent zoomComponent = new();

        private GlobalCamera assignedCamera;
        bool isPaused = false;
        
        
        Transform lastFollowedTarget;
        
        public void Initialize(GlobalCamera globalCamera)
        {
            assignedCamera = globalCamera;
            
            moveComponent.Initialize(assignedCamera);
            rotateComponent.Initialize(assignedCamera);
            dragComponent.Initialize(assignedCamera);
            zoomComponent.Initialize(assignedCamera);
        }
        public void Dispose()
        {
            moveComponent.SetActive(false);
            rotateComponent.SetActive(false);
            dragComponent.SetActive(false);
            zoomComponent.SetActive(false);
        }
        
        public CameraControlFlag GetCurrentMode() => cameraControlFlag;
        public void Pause(PauseLevel pauseLevel)
        {
            isPaused = true;
        }
        public void Unpause()
        {
            isPaused = false;
        }
        
        public void RotateCamera(DirectionFlag direction)
        {
            rotateComponent.Rotate(direction);
        }

        public void SwitchCameraModes()
        {
            if(IsInMode(CameraControlFlag.FreeMode))
            {
                SetFixedMode();
            }
            else if(IsInMode(CameraControlFlag.FixedMode))
            {
                SetFreeMode();
            }
        }

        public Transform GetLastFollowedTarget() => lastFollowedTarget;
        public void SetLastFollowedTarget(Transform target)
        {
            if(target != assignedCamera.CameraGuide.transform) lastFollowedTarget = target;
        }
        void SetFreeMode()
        {
            var cinemachineContainer = assignedCamera.CinemachineContainer;

            if (cinemachineContainer.TryGetCurrentTarget(out var target) &&
                target != assignedCamera.CameraGuide.transform)
            {
                lastFollowedTarget = target;
                assignedCamera.CameraGuide.transform.position = lastFollowedTarget.position;
            }
            
            assignedCamera.CameraGuide.RequestFocus();
            SetFlag(CameraControlFlag.FreeMode);
        }
        void SetFixedMode()
        {
           
            var cameraFocus = lastFollowedTarget?.GetComponent<CameraFocus>();
            Debug.Log("ESTABA SIGUIENDO A: " + lastFollowedTarget?.name);
            if (cameraFocus != null)
            {
                SetFlag(CameraControlFlag.FixedMode);
                cameraFocus?.RequestFocus();
                
                assignedCamera.CameraEvents.OnCameraFollowChanged(lastFollowedTarget);
            }
        }
        public void SetFlag(CameraControlFlag flag)
        {
            cameraControlFlag = flag;
            UpdateComponents();
            assignedCamera.CameraEvents.OnCameraModeModified(flag);
        }
        void UpdateComponents()
        {
            moveComponent.SetActive(HasFlag(CameraControlFlag.Move));
            rotateComponent.SetActive(HasFlag(CameraControlFlag.Rotate));
            dragComponent.SetActive(HasFlag(CameraControlFlag.Drag));
            zoomComponent.SetActive(HasFlag(CameraControlFlag.Zoom));
        }
        bool HasFlag(CameraControlFlag flag) => (cameraControlFlag & flag) == flag;
        public bool IsInMode(CameraControlFlag flag) => cameraControlFlag == flag;
        void Update()
        {
            if(isPaused) return;

            bool isGameOnMobile = runtimeVariableLocator.IsGamePlayedOnMobile;
            
            moveComponent.HandleMove();
            dragComponent.HandleDrag(isGameOnMobile);
            zoomComponent.HandleZoom(isGameOnMobile);
        }
    }
}