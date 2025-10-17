using CursedOnion.Game.Cameras;
using CursedOnion.Game.Settings;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace CursedOnion.UI
{
    public class CanvasCameraFinder : MonoBehaviour
    {
        [SerializeField] private float planeDistance = 0.2f;
        
        [Inject] RuntimeSettings runtimeSettings;
        GlobalCamera globalCamera;
        Canvas canvas;
        void Awake()
        {
            globalCamera = runtimeSettings.GlobalCamera;
            canvas = GetComponent<Canvas>();
            
            canvas.worldCamera = globalCamera.Camera;
            canvas.planeDistance = planeDistance;
        }
    }
}
