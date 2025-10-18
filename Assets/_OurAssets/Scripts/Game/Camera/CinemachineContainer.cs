using Unity.Cinemachine;
using UnityEngine;

namespace CursedOnion.Game.Cameras
{
    [System.Serializable]
    public class CinemachineContainer
    {
        public CinemachineCamera CinemachineCamera;
        public CinemachinePanTilt PanTilt;
        public CinemachineFollow Follow;
        public CinemachineCameraOffset Offset;

        public void SetTarget(Transform target, Vector3 offset)
        {
            CinemachineCamera.Follow = target;
            Offset.Offset = offset;
        }

        public void SetPanCenterAndValue(float panValue)
        {
            PanTilt.PanAxis.Center = panValue;
            PanTilt.PanAxis.Value = panValue;
        }
        public void SetPanCenter(float panValue)
        {
            PanTilt.PanAxis.Center = panValue;
        }

        public void SetTiltCenterAndValue(float tiltValue)
        {
            PanTilt.TiltAxis.Center = tiltValue;
            PanTilt.TiltAxis.Value = tiltValue;
        }

        public void SetTiltCenter(float tiltValue)
        {
            PanTilt.TiltAxis.Center = tiltValue;
        }

        public void MatchWith(CinemachineContainer other)
        {
            this.SetTiltCenterAndValue(other.PanTilt.TiltAxis.Center);
            this.SetPanCenterAndValue(other.PanTilt.PanAxis.Center);
            this.SetTarget(other.CinemachineCamera.Follow, other.Offset.Offset);
        }
    }
}