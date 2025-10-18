using System;
using CursedOnion.Game.Cameras;
using CursedOnion.Locators;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Cameras
{
    public class CameraFocus : MonoBehaviour
    {
        [Inject] CameraLocator cameraLocator;
        
        [SerializeField] private bool focusOnAwake;
        [SerializeField] private Vector3 distanceOffset;
        [SerializeField] private float tiltOnFocus;
        private void Awake()
        {
            if(focusOnAwake) Focus();
        }

        public void Focus()
        {
            CinemachineContainer container = cameraLocator.GlobalCamera.CinemachineContainer;
            container.SetTarget(this.transform, distanceOffset);
            container.SetTiltCenter(tiltOnFocus);
            cameraLocator.GlobalCamera.CameraBehaviours.ChangeToFixedMode();
        }
    }
}
