
using NaughtyAttributes;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.EventSystems;
using CursedOnion.Extensions;
using CursedOnion.Game.Audio;
using CursedOnion.Game.Inputs;
using CursedOnion.Game.Events;
using CursedOnion.Game.Inputs.Camera;
using CursedOnion.Locators;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CursedOnion.Game.Cameras
{
    [RequireComponent(typeof(CameraController))]
    public class GlobalCamera : MonoBehaviour
    {
        [Inject] RuntimeVariableLocator runtimeVariableLocator;
        [Inject] InputReaderCollection inputReaderCollection;
        [HideInInspector] public CameraInputReader CameraInputReader;
        
        [BoxGroup("UI Interactions"), SerializeField] private EventSystem eventSystem;
        
        [BoxGroup("Default Camera Variables")] public Camera Camera;
        [BoxGroup("Default Camera Variables")] public Camera UiCamera;
        [BoxGroup("Default Camera Variables"), SerializeField] private AudioListener audioListener; 
        
        [BoxGroup("Camera Controls"),SerializeField] public CameraFocus CameraGuide;
        [BoxGroup("Camera Controls"),SerializeField] public CameraController CameraController;
        
        [BoxGroup("Cinemachine"),SerializeField] public CinemachineContainer CinemachineContainer;
        
        public CameraEvents CameraEvents;

        #region Initialization & Destruction
        void Awake()
        {
            var instancedCamera = runtimeVariableLocator.GlobalCamera;
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
            runtimeVariableLocator.GlobalCamera = this;
            
            eventSystem.enabled = true;
            audioListener.enabled = true;
            
            CameraInputReader = inputReaderCollection.GetReader<CameraInputReader>();
                
            CameraController.Initialize(this);
            
            GetComponent<MusicPlayer>()?.StartMusic();
            
            var pipelineAsset = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
            pipelineAsset.EnableRenderFeature<FullScreenPassRendererFeature>(false);
        }
        void MatchWith(GlobalCamera other)
        {
            transform.position = other.transform.position;
            transform.rotation = other.transform.rotation;
            
            CinemachineContainer.MatchWith(other.CinemachineContainer);
            CameraController.Unpause();
        }
        
        void OnDisable()
        {
            var instancedCamera = runtimeVariableLocator.GlobalCamera;
            if (instancedCamera != null && instancedCamera == this)
            {
                runtimeVariableLocator.GlobalCamera = null;
                CameraController.Dispose();
            }
        }
        #endregion
        
        public void SwitchCameraModes()
        {
            CameraController.SwitchCameraModes();
        }
        public void SetCameraMode(CameraControlFlag flag)
        {
            CameraController.SetFlag(flag);
        }
        
        public void FocusOn(Transform target, Vector3 positionDamping, float tiltOnFocus)
        {
            CameraController.SetLastFollowedTarget(target);

            if (!CameraController.IsInMode(CameraControlFlag.FreeMode))
            {
                CinemachineContainer.FocusOn(target, positionDamping, tiltOnFocus);
                CameraEvents.OnCameraFollowChanged(target);
            }
        }
        public float GetCameraPanAngles() => CinemachineContainer.GetCameraPanAngles();
        
    }
}
