using CursedOnion.Game.Logic;
using CursedOnion.Game.Objects;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.UI.Canvases
{
    public class MapUICanvas : MonoBehaviour, IUICanvas
    {
        [Inject] MediatorEvents mediatorEvents;
        
        [SerializeField] private GameObject PlayLevelButton;
        void Awake()
        {
            mediatorEvents.OnLevelInspectionChange += UpdateMapCanvas;
        }
        void OnDestroy()
        {
            mediatorEvents.OnLevelInspectionChange -= UpdateMapCanvas;
        }

        void UpdateMapCanvas(LevelPlatform inspectedLevel)
        {
            PlayLevelButton.SetActive(!inspectedLevel.IsEmptyLevel());
        }
    }
}