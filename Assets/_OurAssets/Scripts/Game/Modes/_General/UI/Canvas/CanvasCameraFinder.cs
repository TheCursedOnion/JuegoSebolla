using CursedOnion.Game.Cameras;
using CursedOnion.Game.Settings;
using CursedOnion.Locators;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace CursedOnion.Game.General.UI.Canvases
{
    public class CanvasCameraFinder : MonoBehaviour
    {
        [SerializeField] private float planeDistance = 0.2f;
        
        [Inject] RuntimeVariableLocator runtimeVariableLocator;
        [SerializeField] RenderMode renderMode;
        GlobalCamera globalCamera;
        Canvas canvas;
        void Awake()
        {
            globalCamera = runtimeVariableLocator.GlobalCamera;
            canvas = GetComponent<Canvas>();
            
            canvas.worldCamera = globalCamera.Camera;
            canvas.planeDistance = planeDistance;
            canvas.renderMode = renderMode;
        }
    }
}
