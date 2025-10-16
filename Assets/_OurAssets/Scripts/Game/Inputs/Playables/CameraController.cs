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
        float GetCameraPanAngles() => cinemachineContainer.CinemachinePanTilt.PanAxis.Center;
        

        public void Initialize(CinemachineContainer cinemachineContainer)
        {
            this.cinemachineContainer = cinemachineContainer;
        }
        public void Enable()
        {
            reader = InputReaderCollection.GetReader<CameraInputReader>();
            Debug.Log(reader != null);
        }
        public void Disable()
        {
            if (reader != null)
            {
                reader.Move -= MoveCamera;
                reader.RotateCamera -= RotateCamera;
            }
        }
        
        public void EnableMove(bool enable)
        {
            
            if(enable)
                reader.Move += MoveCamera;
            else
                reader.Move -= MoveCamera;
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
            cinemachineContainer.CinemachineFollow.enabled = enable;
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
            float rotateAmount = direction == DirectionFlag.Left ? -45 : 45;

            var cinemachinePanTilt = cinemachineContainer.CinemachinePanTilt;
            
            cinemachinePanTilt.PanAxis.Center += rotateAmount;
            
            cinemachinePanTilt.PanAxis.Center %= 360f;
            if (cinemachinePanTilt.PanAxis.Center < 0)
                cinemachinePanTilt.PanAxis.Center += 360f;
        }

        void Update()
        {
            transform.position += moveDir * (moveSpeed * Time.deltaTime);
        }
    }
}