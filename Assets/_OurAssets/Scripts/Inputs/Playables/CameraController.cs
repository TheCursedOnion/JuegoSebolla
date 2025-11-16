using System;
using CursedOnion.Extensions;
using CursedOnion.Game;
using CursedOnion.Game.Cameras;
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
        [SerializeField] private float moveSpeed;
        [Inject] public InputReaderCollection InputReaderCollection { get; set; }
        private CameraInputReader reader;
        
        private CinemachineContainer cinemachineContainer;
        float GetCameraPanAngles() => cinemachineContainer.PanTilt.PanAxis.Center;
        

        public void Initialize(GlobalCamera globalCamera)
        {
            this.cinemachineContainer = globalCamera.CinemachineContainer;
            reader = InputReaderCollection.GetReader<CameraInputReader>();
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
            EnableFollow(false);
            EnableMove(false);
            EnableRotate(false);
        }
        public void Pause() => reader.Disable();
        public void Unpause() => reader.Enable();
        
        bool canMove = true;
        public void EnableMove(bool enable)
        {
            canMove = enable;
            //if(!canMove) moveDir = Vector3.zero;
        }
        public void EnableRotate(bool enable)
        {
            if(enable)
                reader.RotateCamera += RotateCamera;
            else
                reader.RotateCamera -= RotateCamera;
        }
        
        Transform lastFollowedTarget;

        public void EnableFollow(bool enable)
        {
            var cam = cinemachineContainer.CinemachineCamera;
            var target = cam.Follow;
            if (!enable && target != null)
            {
                lastFollowedTarget = target;
                cam.Follow = null;
            }
            else
            {
                if (lastFollowedTarget != null)
                {
                    cam.ForceCameraPosition(cam.transform.position, cam.transform.rotation);
                    cam.Follow = lastFollowedTarget;
                }
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
           if(canMove) transform.position += moveDir * (moveSpeed * Time.deltaTime);
        }
    }
}