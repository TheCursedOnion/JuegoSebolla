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

            float rotateAngle = cinemachineContainer.GetCameraPanAngles();
            Quaternion rotation = Quaternion.AngleAxis(rotateAngle, Vector3.up);
            direction3D = rotation * direction3D;
            
            moveDir = cameraFreeGuide.forward * direction3D.z + cameraFreeGuide.right * direction3D.x;
        }
        public void HandleMove()
        {
            
            if(!enabled) return;
            cameraFreeGuide.position += moveDir * (moveSpeed * Time.deltaTime);
        }
    }
}