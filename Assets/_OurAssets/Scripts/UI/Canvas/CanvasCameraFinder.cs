using CursedOnion.Game.Cameras;
using CursedOnion.Game.Settings;
using CursedOnion.Locators;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace CursedOnion.UI.Canvases
{
    public class CanvasCameraFinder : MonoBehaviour
    {
        [SerializeField] private float planeDistance = 0.2f;
        
        [Inject] CameraLocator cameraLocator;
        GlobalCamera globalCamera;
        Canvas canvas;
        void Awake()
        {
            globalCamera = cameraLocator.GlobalCamera;
            canvas = GetComponent<Canvas>();
            
            canvas.worldCamera = globalCamera.Camera;
            canvas.planeDistance = planeDistance;
        }
    }
}
