using CursedOnion.Game.Cameras;
using CursedOnion.Locators;
using NaughtyAttributes;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Inputs
{
    public class ZoomController : MonoBehaviour
    {
        [Inject] RuntimeVariableLocator variableLocator;
        
        [Header("Sensitivity")]
        [SerializeField] float scrollSensitivity = 4f;
        [SerializeField] float pinchSensitivity = 0.08f;
        [SerializeField] float smoothZoomSpeed = 10f;
        CinemachineContainer cinemachineContainer;
        
        public void Initialize(CinemachineContainer cinemachineContainer)
        {
            this.cinemachineContainer = cinemachineContainer;
        }
        
        public void HandleZoom()
        {
            if(!variableLocator.IsGamePlayedOnMobile)
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
                cinemachineContainer.AddFollowOffsetZ(zoomZ, smoothZoomSpeed * Time.deltaTime);
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
                cinemachineContainer.AddFollowOffsetZ(zoomZ, smoothZoomSpeed * Time.deltaTime);
            
        }
    }
}