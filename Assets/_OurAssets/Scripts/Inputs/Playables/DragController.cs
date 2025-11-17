using System;
using CursedOnion.Locators;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CursedOnion.Game.Inputs
{
    public class DragController : MonoBehaviour
    { 
        [Inject] RuntimeVariableLocator variableLocator;
        public float dragSpeed = 0.1f;
        public float damping = 10f;

        private Vector3 dragOrigin;
        private Vector3 targetPosition;
        
        private bool dragStartedOnUI = true;
        private bool isDragging = false;

        private Transform targetGuide;

        public void Initialize(Transform guide)
        {
            targetGuide = guide;
            targetPosition = guide.position;
        }

        public void HandleDrag()
        {
            if(!variableLocator.IsGamePlayedOnMobile)
                HandleMouseDrag();
            else
                HandleTouchDrag();

            if (isDragging)
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * damping);
            
        }
        void HandleMouseDrag()
        {
            if (Input.GetMouseButtonDown(0))
            {
                dragStartedOnUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
                dragOrigin = Input.mousePosition;
                targetPosition = transform.position;
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
            if (Input.touchCount == 0) return;
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                dragStartedOnUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId);
                dragOrigin = touch.position;
                targetPosition = transform.position;
            }
            
            isDragging = touch.phase == TouchPhase.Moved;
            if (isDragging && !dragStartedOnUI)
            {
                var dragOrigin2D = new Vector2(dragOrigin.x, dragOrigin.y);
                Vector3 delta = touch.position - dragOrigin2D;
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

            targetPosition += worldMove;
        }

    }
}