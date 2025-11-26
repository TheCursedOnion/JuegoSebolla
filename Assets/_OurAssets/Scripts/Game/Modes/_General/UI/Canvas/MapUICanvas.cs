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

        void DisableAll()
        {
            settingsContainer.SetActive(false);
            gameplayContainer.SetActive(false);
        }
        void EnableOnly(GameObject screen)
        {
            DisableAll();
            screen.SetActive(true);
        }
        #region Settings Region
        public void Pause(PauseLevel pauseLevel)
        {
            switch (pauseLevel)
            {
                case PauseLevel.Dialog: DisableAll(); break;
                case PauseLevel.UI: EnableOnly(settingsContainer); break;
            }
        }
        public void Unpause()
        {
            EnableOnly(gameplayContainer);
        }
        #endregion
    }
}