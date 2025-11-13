using CursedOnion.Game.Cameras;
using CursedOnion.Locators;
using Reflex.Extensions;
using UnityEngine;

namespace CursedOnion
{
    public class FollowCamera : MonoBehaviour
    {
        private GlobalCamera globalCamera;
        
        void Start()
        {
            globalCamera = gameObject.scene.GetSceneContainer().Resolve<CameraLocator>().GlobalCamera;
        }

        void Update()
        {
            float yRotation = globalCamera.CinemachineContainer.PanTilt.PanAxis.Value;
            this.transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
        }
    }
}
