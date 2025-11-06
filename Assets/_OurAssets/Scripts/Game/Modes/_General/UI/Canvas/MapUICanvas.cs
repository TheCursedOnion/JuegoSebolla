using CursedOnion.Game.Events;
using CursedOnion.Game.Logic;
using CursedOnion.Game.Objects;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.General.UI.Canvases
{
    public class MapUICanvas : MonoBehaviour, IUICanvas
    {
        [Inject] MapEvents mapEvents;
        
        [SerializeField] private GameObject PlayLevelButton;
        void Awake()
        {
            mapEvents.OnLevelPlatformChange += UpdateMapCanvas;
        }
        void OnDestroy()
        {
            mapEvents.OnLevelPlatformChange -= UpdateMapCanvas;
        }

        void UpdateMapCanvas(LevelPlatform inspectedLevel)
        {
            PlayLevelButton.SetActive(!inspectedLevel.IsEmptyLevel());
        }
    }
}