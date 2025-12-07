using System;
using CursedOnion.Game.Cameras;
using CursedOnion.Locators;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CursedOnion.Game.Inputs.Camera
{
    [System.Serializable]
    public class DragComponent
    {
        [SerializeField] private float dragSpeed = 0.1f;
        [SerializeField] private float damping = 10f;

        private Vector3 dragOrigin;
        
        private bool dragStartedOnUI = true;
        private bool isDragging = false;
        private int? activeDragFingerId = null;
        
        private Transform targetGuide;
        private Vector3 targetPosition;
        CinemachineContainer cinemachineContainer;
        
        bool enabled = true;
        public void Initialize(GlobalCamera camera)
        {
            cinemachineContainer = camera.CinemachineContainer;
            targetGuide = camera.CameraGuide.transform;
            targetPosition = targetGuide.position;
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
        public void HandleDrag(bool useMobileControls)
        {
            if(!enabled) return;
            
            if(!useMobileControls)
                HandleMouseDrag();
            else
                HandleTouchDrag();

            if (isDragging)
                targetGuide.position = Vector3.Lerp(targetGuide.position, targetPosition, Time.deltaTime * damping);
            
        }
        void HandleMouseDrag()
        {
            if (Input.GetMouseButtonDown(0))
            {
                dragStartedOnUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
                dragOrigin = Input.mousePosition;
                targetPosition = targetGuide.position;
            }

            isDragging = Input.GetMouseButton(0);
            if (isDragging && !dragStartedOnUI)
            {
                Vector3 delta = Input.mousePosition - dragOrigin;
                dragOrigin = Input.mousePosition;

                DragCamera(delta);
            }
        }
        
        void HandleTouchDrag()
        {
            if (Input.touchCount == 0) 
            {
                isDragging = false;
                activeDragFingerId = null;
                return;
            }
            if (Input.touchCount > 1)
            {
                isDragging = false;
                activeDragFingerId = null;
                return;
            }

            Touch touch = Input.GetTouch(0);
            
            if (touch.phase == TouchPhase.Began)
            {
                dragStartedOnUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId);
                dragOrigin = touch.position;
                targetPosition = targetGuide.position;

                activeDragFingerId = touch.fingerId;
            }
            
            if (activeDragFingerId != touch.fingerId) return;
            
            isDragging = touch.phase == TouchPhase.Moved;

            if (isDragging && !dragStartedOnUI)
            {
                var dragOrigin2D = new Vector2(dragOrigin.x, dragOrigin.y);
                Vector3 delta = (Vector3)(touch.position - dragOrigin2D);
                dragOrigin = touch.position;

                DragCamera(delta);
            }
        }
        void DragCamera(Vector3 delta)
        {
            Vector3 move = new Vector3(-delta.x, 0, -delta.y) * dragSpeed;
            
            Vector3 right = targetGuide.transform.right;
            Vector3 forward = targetGuide.transform.forward;

            right.y = 0;
            forward.y = 0;

            right.Normalize();
            forward.Normalize();

            Vector3 worldMove = right * move.x + forward * move.z;
            
            worldMove = AdjustDirectionToRotation(worldMove);
            targetPosition += worldMove;
        }
        Vector3 AdjustDirectionToRotation(Vector3 direction)
        {
            float rotateAngle = cinemachineContainer.GetCameraPanAngles();
            Quaternion rotation = Quaternion.AngleAxis(rotateAngle, Vector3.up);
            Vector3 fixedRotation = rotation * direction;
            return targetGuide.forward * fixedRotation.z + targetGuide.right * fixedRotation.x;
        }

    }
}