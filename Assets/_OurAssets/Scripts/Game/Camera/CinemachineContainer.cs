using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace CursedOnion.Game.Cameras
{
    [System.Serializable]
    public class CinemachineContainer
    {
        public MonoBehaviour CoroutineExecuter;
        
        public CinemachineCamera CinemachineCamera;
        public CinemachinePanTilt PanTilt;
        public CinemachineFollow Follow;
        public CinemachineCameraOffset Offset;
        
        public void FocusOn(Transform target, Vector3 offset, float tiltOnFocus, float time = 0f)
        {
            SetTarget(target, offset, time);
            SetPanCenter(target.transform.eulerAngles.y);
            SetTiltCenter(tiltOnFocus);
        }
        public void SetTarget(Transform target, Vector3 offset, float adjustTime)
        {
            CinemachineCamera.Follow = target;
            CoroutineExecuter.StopAllCoroutines();
            CoroutineExecuter.StartCoroutine(IEOffset(offset, adjustTime));
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
            this.SetTarget(other.CinemachineCamera.Follow, other.Offset.Offset, 0f);
        }

        void LerpOffset(Vector3 offset, float time)
        {
            Offset.Offset = Vector3.Lerp(Offset.Offset, offset, time);
        }

        IEnumerator IEOffset(Vector3 offset, float time)
        {
            float elpasedTime = 0f;
            while (elpasedTime < time)
            {
                LerpOffset(offset, elpasedTime / time);
                elpasedTime += Time.deltaTime;
                yield return null;
            }

            Offset.Offset = offset;
        }
    }
}