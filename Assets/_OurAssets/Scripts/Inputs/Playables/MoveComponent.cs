using System;
using CursedOnion.Extensions;
using CursedOnion.Game.Cameras;
using UnityEngine;

namespace CursedOnion.Game.Inputs.Camera
{
    [System.Serializable]
    public class MoveComponent
    {
        [SerializeField] private float moveSpeed;
        private CameraInputReader reader;
        CinemachineContainer cinemachineContainer;
        Transform cameraFreeGuide;
        
        Vector3 moveDir;
        bool enabled = true;
        public void Initialize(GlobalCamera camera)
        {
            this.cinemachineContainer = camera.CinemachineContainer;
            this.cameraFreeGuide = camera.CameraGuide.transform;
            this.reader = camera.CameraInputReader;
            
            Enable();
        }

        public void SetActive(bool active)
        {
            enabled = active;
            
            if (enabled) Enable();
            else Disable();
        }
        void Enable()
        {
            Disable();
            reader.Move += ProcessMove;
        }
        void Disable()
        {
            reader.Move -= ProcessMove;
            moveDir = Vector3.zero;
        }
        void ProcessMove(Vector2 direction)
        {
            Vector3 direction3D = direction;
            direction3D = direction3D.SwizzleXZY();
            
            moveDir = direction3D;
        }
        
        Vector3 AdjustDirectionToRotation(Vector3 direction)
        {
            float rotateAngle = cinemachineContainer.GetCameraPanAngles();
            Quaternion rotation = Quaternion.AngleAxis(rotateAngle, Vector3.up);
            Vector3 fixedRotation = rotation * direction;
            return cameraFreeGuide.forward * fixedRotation.z + cameraFreeGuide.right * fixedRotation.x;
        }
        public void HandleMove()
        {
            
            if(!enabled) return;
            cameraFreeGuide.position += AdjustDirectionToRotation(moveDir) * (moveSpeed * Time.deltaTime);
        }
    }
}