using System;
using CursedOnion.Game.Inputs;
using CursedOnion.Game.Settings;
using Reflex.Attributes;
using Reflex.Core;
using Reflex.Extensions;
using Unity.Cinemachine;
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
        
        [FormerlySerializedAs("cameraPlayable")] [SerializeField] Inputs.CameraController CameraController;
        
        [SerializeField] CinemachineContainer cinemachineContainer;
        public CinemachineContainer CinemachineContainer => cinemachineContainer;
        public float GetCameraPanAngles() => cinemachineContainer.CinemachinePanTilt.PanAxis.Center;

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
            
            CameraController.Initialize(cinemachineContainer);
            CameraController.Enable();
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
                
                CameraController.Disable();
            }
        }
        #endregion
    }

    [System.Serializable]
    public class CinemachineContainer
    {
        public CinemachineCamera CinemachineCamera;
        public CinemachinePanTilt CinemachinePanTilt;
        public CinemachineFollow CinemachineFollow;
    }
}
