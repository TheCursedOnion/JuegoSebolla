using System;
using CursedOnion.Game.Cameras;
using CursedOnion.Locators;
using NaughtyAttributes;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Cameras
{
    public class CameraFocus : MonoBehaviour
    {
        [Inject] CameraLocator cameraLocator;
        
        [SerializeField] private bool focusOnAwake;

        [SerializeField] private Vector3 positionDamping = Vector3.one;
        [SerializeField] private Vector3 distanceOffset;
        [SerializeField] private float tiltOnFocus;
        
        [SerializeField] private float focusTime = 1f;
        private void Awake()
        {
            if(focusOnAwake) Focus();
        }
        
        [Button]
        public void Focus()
        {
            cameraLocator.GlobalCamera.FocusCameraOn(this.transform, positionDamping, distanceOffset, tiltOnFocus, focusTime);
        }
    }
}
