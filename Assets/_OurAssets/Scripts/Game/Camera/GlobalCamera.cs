using System;
using CursedOnion.Behaviours;
using CursedOnion.Game.Inputs;
using CursedOnion.Game.Settings;
using CursedOnion.Locators;
using NaughtyAttributes;
using Reflex.Attributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

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

        #region Initialization & Destruction
        void Awake()
        {
            var instancedCamera = cameraLocator.GlobalCamera;
            if (instancedCamera != null && instancedCamera != this)
            {
                instancedCamera.PlaceTransform(this.transform);
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
            
            cameraController.Initialize(cinemachineContainer);
            cameraController.Enable();
            
            cameraBehaviours.Initialize(cameraController);
        }
        void PlaceTransform(Transform other)
        {
            transform.position = other.position;
            transform.rotation = other.rotation;
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
