using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.Cinemachine;
using UnityEngine;

namespace CursedOnion.Game.Cameras
{
    [System.Serializable]
    public class CinemachineContainer : MonoBehaviour
    {
        public CinemachineCamera CinemachineCamera;
        public CinemachinePanTilt PanTilt;
        public CinemachineFollow Follow;
        public CinemachineCameraOffset Offset;

        #region Getters

        public float GetCameraPanAngles() => PanTilt.PanAxis.Center;
        public float GetCameraTiltAngles() => PanTilt.TiltAxis.Center;
        public float GetCameraFollowOffsetZ() => Offset.Offset.z;

        public bool TryGetCurrentTarget(out Transform target)
        {
            target = CinemachineCamera.Follow;
            return target != null;
        }

        #endregion
        
        public void FocusOn(Transform target, Vector3 positionDamping, float tiltOnFocus)
        {
            SetTarget(target, positionDamping);
        }
        public void SetTarget(Transform target, Vector3 positionDamping)
        {
            Follow.TrackerSettings.PositionDamping = positionDamping;
            CinemachineCamera.Follow = target;
        }
        public void SetOffset(Vector3 offset, float adjustTime)
        {
            StopAllCoroutines();
            StartCoroutine(IEOffset(offset, adjustTime));
        }
        public void SetFollowOffsetZ(float zOffset, float smoothTime)
        {
            var offset = Offset.Offset;
            offset.z = zOffset;
            StartCoroutine(IEOffset(offset, smoothTime));
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
            this.SetTarget(other.CinemachineCamera.Follow, other.Follow.TrackerSettings.PositionDamping);
            this.SetOffset(other.Offset.Offset, 0f);
        }
        
        IEnumerator IEOffset(Vector3 offset, float time)
        {
            float elapsed = 0f;
            Vector3 startOffset = Offset.Offset;
            while (elapsed < time)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / time);
                
                Offset.Offset = Vector3.Lerp(startOffset, offset, t);
                yield return null;
            }

            Offset.Offset = offset;
        }
    }
}