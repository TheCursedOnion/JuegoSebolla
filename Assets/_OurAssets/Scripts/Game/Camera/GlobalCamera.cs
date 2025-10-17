using System;
using CursedOnion.Behaviours;
using CursedOnion.Game.Inputs;
using CursedOnion.Game.Settings;
using NaughtyAttributes;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Cameras
{
    [RequireComponent(typeof(CameraController))]
    public class GlobalCamera : MonoBehaviour
    {
        [Inject] RuntimeSettings runtimeSettings;
        
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
            var instancedCamera = runtimeSettings.GlobalCamera;
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
            runtimeSettings.GlobalCamera = this;
            
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
            var instancedCamera = runtimeSettings.GlobalCamera;
            if (instancedCamera != null && instancedCamera == this)
            {
                runtimeSettings.GlobalCamera = null;
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
