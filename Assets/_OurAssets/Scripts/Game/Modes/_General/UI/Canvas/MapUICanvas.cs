using CursedOnion.Game.Events;
using CursedOnion.Game.Logic;
using CursedOnion.Game.Logic.Services.Pause;
using CursedOnion.Game.Objects;
using NaughtyAttributes;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.General.UI.Canvases
{
    public class MapUICanvas : MonoBehaviour, IUICanvas, IPausable
    {
        const string SettingsContainer = "Settings Container Variables";
        const string GameplayContainer = "Gameplay Container Variables";
        const string CameraContainer = "Camera Container Variables";
        
        //[SerializeField, BoxGroup(CameraContainer)] private GameObject cameraButtonsContainer;
        [SerializeField, BoxGroup(SettingsContainer)] private GameObject settingsContainer;
        [SerializeField, BoxGroup(GameplayContainer)] private GameObject gameplayContainer;

        #region Settings Region
        public void Pause()
        {
            settingsContainer.SetActive(true);
            gameplayContainer.SetActive(false);
        }

        public void Unpause()
        {
            settingsContainer.SetActive(false);
            gameplayContainer.SetActive(true);
        }
        #endregion
    }
}