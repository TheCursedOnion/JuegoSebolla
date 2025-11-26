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
        [Inject] RuntimeVariableLocator runtimeVariableLocator;
        
        [SerializeField] bool isUI = false;
        [SerializeField] private float planeDistance = 0.2f;
        
        [SerializeField] RenderMode renderMode;
        [SerializeField] string sortingLayerName = "Default";
        [SerializeField] int sortingOrder = 0;
        
        void Awake()
        {
            GlobalCamera globalCamera = runtimeVariableLocator.GlobalCamera;
            Canvas canvas = GetComponent<Canvas>();
            
            canvas.worldCamera = !isUI ? globalCamera.Camera : globalCamera.UiCamera;
            canvas.planeDistance = planeDistance;
            canvas.renderMode = renderMode;
            canvas.sortingLayerName = sortingLayerName;
            canvas.sortingOrder = sortingOrder;
        }
    }
}
