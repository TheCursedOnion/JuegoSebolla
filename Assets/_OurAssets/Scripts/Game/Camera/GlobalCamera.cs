using System;
using NaughtyAttributes;
using Reflex.Attributes;

using UnityEngine;
using UnityEngine.EventSystems;

using CursedOnion.Behaviours;
using CursedOnion.Game.Inputs;
using CursedOnion.Game.Events;
using CursedOnion.Locators;

namespace CursedOnion.Game.Cameras
{
    [RequireComponent(typeof(CameraController))]
    public class GlobalCamera : MonoBehaviour
    {
        [Inject] CameraLocator cameraLocator;
        
        [BoxGroup("UI Interactions"), SerializeField] private EventSystem eventSystem;
        
        [BoxGroup("Default Camera Variables")] public Camera Camera;
        [BoxGroup("Default Camera Variables"), SerializeField] private AudioListener audioListener; 
        
        [BoxGroup("Camera Controls"),SerializeField] CameraController cameraController;
        public CameraController CameraController => cameraController;
        
        [BoxGroup("Camera Controls"),SerializeField] CameraBehaviours cameraBehaviours;
        public CameraBehaviours CameraBehaviours => cameraBehaviours;
        
        
        [BoxGroup("Cinemachine"),SerializeField] CinemachineContainer cinemachineContainer;
        public CinemachineContainer CinemachineContainer => cinemachineContainer;
        public float GetCameraPanAngles() => cinemachineContainer.PanTilt.PanAxis.Center;
        
        public CameraEvents CameraEvents;

        #region Initialization & Destruction
        void Awake()
        {
            var instancedCamera = cameraLocator.GlobalCamera;
            if (instancedCamera != null && instancedCamera != this)
            {
                instancedCamera.MatchWith(this);
                Destroy(gameObject);
            }
            else
            {
                Initialize();
            }
        }
        void Initialize()
        {
            DontDestroyOnLoad(gameObject);
            cameraLocator.GlobalCamera = this;
            
            eventSystem.enabled = true;
            audioListener.enabled = true;
            
            CameraEvents = new CameraEvents(true);
            

            cameraBehaviours.Initialize(this);
            cameraController.Initialize(this);
            cameraController.Enable();
        }
        void MatchWith(GlobalCamera other)
        {
            transform.position = other.transform.position;
            transform.rotation = other.transform.rotation;
            
            cinemachineContainer.MatchWith(other.cinemachineContainer);
        }
        void OnDisable()
        {
            var instancedCamera = cameraLocator.GlobalCamera;
            if (instancedCamera != null && instancedCamera == this)
            {
                cameraLocator.GlobalCamera = null;
                cameraController.Disable();
            }
        }
        #endregion
        
        public void SwitchCameraModes()
        {
            cameraBehaviours.SwitchCameraModes();
        }
    }
}
