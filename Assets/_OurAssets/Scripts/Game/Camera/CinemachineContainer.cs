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
        [MinMaxSlider(-20f, -3f), SerializeField] private Vector2 zoomLimits;
        
        public CinemachineCamera CinemachineCamera;
        public CinemachinePanTilt PanTilt;
        public CinemachineFollow Follow;
        public CinemachineCameraOffset Offset;
        
        public void FocusOn(Transform target, Vector3 positionDamping, Vector3 offset, float tiltOnFocus, float time = 0f)
        {
            SetTarget(target, positionDamping, offset, time);
            SetPanCenter(target.transform.eulerAngles.y);
            SetTiltCenter(tiltOnFocus);
        }
        public void SetTarget(Transform target, Vector3 positionDamping, Vector3 offset, float adjustTime)
        {
            Follow.TrackerSettings.PositionDamping = positionDamping;
            CinemachineCamera.Follow = target;
            StopAllCoroutines();
            SetOffset(offset, adjustTime);
        }
        
        public void SetOffset(Vector3 offset, float adjustTime)
        {
            //Follow.FollowOffset = offset;
            StartCoroutine(IEOffset(offset, adjustTime));
        }

        public void AddFollowOffsetZ(float zOffset, float smoothTime)
        {
            SetFollowOffsetZ(Offset.Offset.z + zOffset, smoothTime);
        }
        public void SetFollowOffsetZ(float zOffset, float smoothTime)
        {
            zOffset = Mathf.Clamp(zOffset, zoomLimits.x, zoomLimits.y);
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
            this.SetTarget(other.CinemachineCamera.Follow, other.Follow.TrackerSettings.PositionDamping, other.Offset.Offset, 0f);
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