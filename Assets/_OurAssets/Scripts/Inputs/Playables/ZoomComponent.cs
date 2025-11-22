using CursedOnion.Game.Cameras;
using CursedOnion.Locators;
using NaughtyAttributes;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Inputs.Camera
{
    [System.Serializable]
    public class ZoomComponent
    {
        [MinMaxSlider(-30f, -3f), SerializeField] private Vector2 zoomLimits;
        
        [Header("Sensitivity")]
        [SerializeField] float scrollSensitivity = 4f;
        [SerializeField] float pinchSensitivity = 0.08f;
        [SerializeField] float smoothZoomSpeed = 10f;
        CinemachineContainer cinemachineContainer;
        bool enabled = true;
        public void Initialize(GlobalCamera camera)
        {
            this.cinemachineContainer = camera.CinemachineContainer;
            Enable();
        }
        
        public void SetActive(bool active)
        {
            if (active) Enable();
            else Disable();
        }
        void Enable()
        {
            enabled = true;
        }
        void Disable()
        {
            enabled = false;
        }
        
        public void HandleZoom(bool useMobileControls)
        {
            if(!enabled) return;
            
            if(!useMobileControls)
                HandleMouseZoom();
            else
                HandleTouchZoom();
        }

        void HandleMouseZoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (Mathf.Abs(scroll) > 0.001f)
            {
                float zoomZ = scroll * scrollSensitivity * 10f;
                ZoomCamera(zoomZ);
            }
        }

        void HandleTouchZoom()
        {
            if (Input.touchCount != 2) return;
            
                Touch t1 = Input.GetTouch(0);
                Touch t2 = Input.GetTouch(1);

                Vector2 prevPos1 = t1.position - t1.deltaPosition;
                Vector2 prevPos2 = t2.position - t2.deltaPosition;

                float prevDistance = Vector2.Distance(prevPos1, prevPos2);
                float currentDistance = Vector2.Distance(t1.position, t2.position);

                float delta = currentDistance - prevDistance;

                float zoomZ = delta * -pinchSensitivity;
                ZoomCamera(zoomZ);
        }

        void ZoomCamera(float zoomZ)
        {
            float currentZoom = cinemachineContainer.GetCameraFollowOffsetZ();
            zoomZ += currentZoom;
            zoomZ = Mathf.Clamp(zoomZ, zoomLimits.x, zoomLimits.y);
            cinemachineContainer.SetFollowOffsetZ(zoomZ, smoothZoomSpeed * Time.deltaTime);
        }
    }
}