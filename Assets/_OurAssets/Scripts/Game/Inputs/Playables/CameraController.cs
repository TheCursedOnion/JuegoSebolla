using System;
using CursedOnion.Extensions;
using CursedOnion.Game;
using CursedOnion.Game.Cameras;
using CursedOnion.Helpers;
using NaughtyAttributes;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CursedOnion.Game.Inputs
{
    [RequireComponent(typeof(GlobalCamera))]
    public class CameraController : MonoBehaviour, IController
    {
        [SerializeField] private float moveSpeed;
        [Inject] public InputReaderCollection InputReaderCollection { get; set; }
        private CameraInputReader reader;
        
        private CinemachineContainer cinemachineContainer;
        float GetCameraPanAngles() => cinemachineContainer.PanTilt.PanAxis.Center;
        

        public void Initialize(CinemachineContainer cinemachineContainer)
        {
            this.cinemachineContainer = cinemachineContainer;
            reader = InputReaderCollection.GetReader<CameraInputReader>();
        }
        public void Enable()
        {
            reader.Move += MoveCamera;
            reader.RotateCamera += RotateCamera;
        }
        public void Disable()
        {
            reader.Move -= MoveCamera;
            reader.RotateCamera -= RotateCamera;
        }
        public void DisableAll()
        {
            EnableFollow(false);
            EnableMove(false);
            EnableRotate(false);
        }
        
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
        public void EnableFollow(bool enable)
        {
            cinemachineContainer.Follow.enabled = enable;
        }

        private Vector3 moveDir;
        void MoveCamera(Vector2 direction)
        {
            Vector3 direction3D = direction;
            direction3D = direction3D.SwizzleXZY();

            float rotateAngle = GetCameraPanAngles();
            Quaternion rotation = Quaternion.AngleAxis(rotateAngle, Vector3.up);
            direction3D = rotation * direction3D;
            
            moveDir = transform.forward * direction3D.z + transform.right * direction3D.x;
        }

        public void RotateLeft()
        {
            RotateCamera(DirectionFlag.Left);
        }
        public void RotateRight()
        {
            RotateCamera(DirectionFlag.Right);
        }
        void RotateCamera(DirectionFlag direction)
        {
            float rotateAmount = direction == DirectionFlag.Left ? 45 : -45;

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