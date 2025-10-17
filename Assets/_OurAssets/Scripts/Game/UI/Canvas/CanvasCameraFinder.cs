using CursedOnion.Game.Cameras;
using CursedOnion.Game.Settings;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace CursedOnion.Game.UI
{
    public class CanvasCameraFinder : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        
        [Inject] RuntimeSettings runtimeSettings;
        GlobalCamera globalCamera;
        void Awake()
        {
            globalCamera = runtimeSettings.GlobalCamera;
        }
        
        
    }
}
