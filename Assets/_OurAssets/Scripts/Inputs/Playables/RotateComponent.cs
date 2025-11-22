using System;
using CursedOnion.Game.Cameras;
using CursedOnion.Helpers;

namespace CursedOnion.Game.Inputs.Camera
{
    [System.Serializable]
    public class RotateComponent
    {
        private CameraInputReader reader;
        CinemachineContainer cinemachineContainer;
        public void Initialize(GlobalCamera camera)
        {
            this.cinemachineContainer = camera.CinemachineContainer;
            this.reader = camera.CameraInputReader;

            Enable();
        }

        public void SetActive(bool active)
        {
            if (active) Enable();
            else Disable();
        }
        void Enable()
        {
            Disable();
            reader.RotateCamera += ProcessRotation;
        }
        void Disable()
        {
            reader.RotateCamera -= ProcessRotation;
        }
        
        public void Rotate(DirectionFlag direction)
        {
            ProcessRotation(direction);
        }
        void ProcessRotation(DirectionFlag direction)
        {
            float rotateAmount = direction == DirectionFlag.Left ? -45 : 45;

            var cinemachinePanTilt = cinemachineContainer.PanTilt;
            
            cinemachinePanTilt.PanAxis.Center += rotateAmount;
            
            cinemachinePanTilt.PanAxis.Center %= 360f;
            if (cinemachinePanTilt.PanAxis.Center < 0)
                cinemachinePanTilt.PanAxis.Center += 360f;
        }
    }
}