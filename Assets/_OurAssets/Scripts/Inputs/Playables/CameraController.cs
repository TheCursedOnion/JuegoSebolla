using System;
using CursedOnion.Behaviours;
using CursedOnion.Extensions;
using CursedOnion.Game;
using CursedOnion.Game.Cameras;
using CursedOnion.Game.Logic.Services;
using CursedOnion.Game.Logic.Services.Pause;
using CursedOnion.Helpers;
using NaughtyAttributes;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CursedOnion.Game.Inputs
{
    [RequireComponent(typeof(GlobalCamera))]
    public class CameraController : MonoBehaviour, IController, IPausable
    {
        [Inject] public InputReaderCollection InputReaderCollection { get; set; }
        
        [SerializeField] private float moveSpeed;

        [SerializeField] private CameraFocus cameraFreeGuide;
        [SerializeField] private DragController dragController;
        [SerializeField] private ZoomController zoomController;
 
        private CameraInputReader reader;
        private CinemachineContainer cinemachineContainer;
        bool isPaused = false;
        
        
        bool hasFreeMode = false;
        Transform lastFollowedTarget;
        float GetCameraPanAngles() => cinemachineContainer.PanTilt.PanAxis.Center;
        
        public void Initialize(GlobalCamera globalCamera)
        {
            this.cinemachineContainer = globalCamera.CinemachineContainer;
            reader = InputReaderCollection.GetReader<CameraInputReader>();
            
            dragController ??= GetComponent<DragController>();
            dragController.Initialize(cameraFreeGuide.transform);
            
            zoomController ??= GetComponent<ZoomController>();
            zoomController.Initialize(cinemachineContainer);
        }
        public void Enable()
        {
            reader.Move += Move;
            reader.RotateCamera += RotateCamera;
        }
        public void Disable()
        {
            reader.Move -= Move;
            reader.RotateCamera -= RotateCamera;
        }
        public void DisableAll()
        {
            hasFreeMode = false;
            EnableRotate(false);
        }

        public void Pause()
        {
            isPaused = true;
            reader.Disable();
        }
        public void Unpause()
        {
            isPaused = false;
            reader.Enable();
        }
        public void EnableRotate(bool enable)
        {
            if(enable)
                reader.RotateCamera += RotateCamera;
            else
                reader.RotateCamera -= RotateCamera;
        }
        
        public void EnableFreeMode()
        {
            Debug.Log("Free mode enabled");
            var cineCam = cinemachineContainer.CinemachineCamera;
            
            hasFreeMode = true;
            if(lastFollowedTarget != null) cameraFreeGuide.transform.position = lastFollowedTarget.position;
            
            lastFollowedTarget = cineCam.Follow;
            cineCam.Follow = cameraFreeGuide.transform;
        }
        public void EnableFixedMode()
        {
            hasFreeMode = false;
            var cineCam = cinemachineContainer.CinemachineCamera;
            
            if (lastFollowedTarget != null)
            {
                var focus = lastFollowedTarget.GetComponent<CameraFocus>();
                if (focus != null)
                {
                    focus.RequestFocus();
                    cameraFreeGuide.transform.position = focus.transform.position;
                }
                cineCam.Follow = lastFollowedTarget;
            }
            else
            {
                lastFollowedTarget = cineCam.Follow;
            }
        }

        private Vector3 moveDir;
        void Move(Vector2 direction)
        {
            Vector3 direction3D = direction;
            direction3D = direction3D.SwizzleXZY();

            float rotateAngle = GetCameraPanAngles();
            Quaternion rotation = Quaternion.AngleAxis(rotateAngle, Vector3.up);
            direction3D = rotation * direction3D;
            
            moveDir = transform.forward * direction3D.z + transform.right * direction3D.x;
        }

        public void RotateCamera(DirectionFlag direction)
        {
            Rotate(direction);
        }
        private void Rotate(DirectionFlag direction)
        {
            float rotateAmount = direction == DirectionFlag.Left ? -45 : 45;

            var cinemachinePanTilt = cinemachineContainer.PanTilt;
            
            cinemachinePanTilt.PanAxis.Center += rotateAmount;
            
            cinemachinePanTilt.PanAxis.Center %= 360f;
            if (cinemachinePanTilt.PanAxis.Center < 0)
                cinemachinePanTilt.PanAxis.Center += 360f;
        }

        void Update()
        {
            if(isPaused) return;
            
            if (hasFreeMode)
            {
                cameraFreeGuide.transform.position += moveDir * (moveSpeed * Time.deltaTime);
                dragController.HandleDrag();
            }
            zoomController.HandleZoom();
        }
    }
}