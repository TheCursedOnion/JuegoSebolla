using System;
using CursedOnion.Behaviours;
using CursedOnion.Game.Inputs;
using CursedOnion.Game.Settings;
using Reflex.Attributes;
using Reflex.Core;
using Reflex.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace CursedOnion.Game.Cameras
{
    [RequireComponent(typeof(CameraController))]
    public class GlobalCamera : MonoBehaviour
    {
        [Inject] RuntimeSettings runtimeSettings;
        
        public Camera Camera;
        
        [SerializeField] Inputs.CameraController cameraController;
        public Inputs.CameraController CameraController => cameraController;
        
        
        [SerializeField] CinemachineContainer cinemachineContainer;
        public CinemachineContainer CinemachineContainer => cinemachineContainer;
        public float GetCameraPanAngles() => cinemachineContainer.PanTilt.PanAxis.Center;
        
        [SerializeField] CameraBehaviours cameraBehaviours;
        public CameraBehaviours CameraBehaviours => cameraBehaviours;

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
    }
}
