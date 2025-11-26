using System;
using CursedOnion.Game.Cameras;
using CursedOnion.Game.Inputs.Camera;
using CursedOnion.Locators;
using NaughtyAttributes;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.Rendering;

namespace CursedOnion.Game.Cameras
{
    public class CameraFocus : MonoBehaviour
    {
        const string FOCUS_VARIABLES = "CameraFocusVariables";
        [Inject] RuntimeVariableLocator runtimeVariableLocator;

        [SerializeField] private bool imposeResultingMode;
        [SerializeField] private CameraControlFlag resultingMode;
        
        [SerializeField] private bool focusOnAwake;
        
        [SerializeField] private Transform target;
        [SerializeField, BoxGroup(FOCUS_VARIABLES)] private Vector3 positionDamping = Vector3.one;
        
        [SerializeField, BoxGroup(FOCUS_VARIABLES)] private bool forceOffset = true;
        [SerializeField, BoxGroup(FOCUS_VARIABLES), ShowIf("forceOffset")] private Vector3 distanceOffset;
        [SerializeField, BoxGroup(FOCUS_VARIABLES), ShowIf("forceOffset")] private float offsetTime = 1f;
        
        [SerializeField, BoxGroup(FOCUS_VARIABLES)] private bool forceTilt = true;
        [SerializeField, BoxGroup(FOCUS_VARIABLES), ShowIf("forceTilt")] private float tiltOnFocus;
        
        [SerializeField, BoxGroup(FOCUS_VARIABLES)] private bool forcePan = true;
        [SerializeField, BoxGroup(FOCUS_VARIABLES), ShowIf("forcePan")] private float panOnFocus;
        private void Awake()
        {
            if (target == null) target = transform;

            if (focusOnAwake) RequestFocus();
        }
        
        [Button]
        public void RequestFocus()
        {
            var camera = runtimeVariableLocator.GlobalCamera;
            if(camera == null || target == null) return;
            
            var cinemachineContainer = camera.CinemachineContainer;
            
            if(imposeResultingMode) camera.SetCameraMode(resultingMode);
            camera.FocusOn(target, positionDamping, tiltOnFocus);
            
            
            if(forceOffset) cinemachineContainer.SetOffset(distanceOffset, offsetTime);
            if (forceTilt) cinemachineContainer.SetTiltCenter(tiltOnFocus);
            if(forcePan) cinemachineContainer.SetPanCenter(panOnFocus);
        }
    }
}
