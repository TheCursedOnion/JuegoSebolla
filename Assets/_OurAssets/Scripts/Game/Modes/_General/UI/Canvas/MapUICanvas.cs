using Ami.BroAudio;
using CursedOnion.Extensions;
using CursedOnion.Game.Audio;
using CursedOnion.Game.Logic.Services.Pause;
using CursedOnion.Locators;
using NaughtyAttributes;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.General.UI.Canvases
{
    public class MapUICanvas : MonoBehaviour, IUICanvas, IPausable
    {
        const string SettingsContainer = "Settings Container Variables";
        const string MapContainer = "Map Container Variables";
        
        [Inject] RuntimeVariableLocator variableLocator;
        [Inject] AudioGallery audioGallery;
        [SerializeField] private SoundID musicToPlay;
        
        [SerializeField] float fadeTime = 0.5f;
        [SerializeField, BoxGroup(SettingsContainer)] private CanvasGroup settingsGroup;
        [SerializeField, BoxGroup(MapContainer)] private CanvasGroup mapGroup;
        
        void DisableAllGroups()
        {
            settingsGroup.SetGroupActive(false, 0f);
            mapGroup.SetGroupActive(false, 0f);
        }
        void EnableOnlyGroup(CanvasGroup container)
        {
            DisableAllGroups();
            container.SetGroupActive(true, fadeTime);
        }
        void EnableOnlyGroups(params CanvasGroup[] container)
        {
            DisableAllGroups();
            foreach (var c in container) c.SetGroupActive(true, fadeTime);
        }
        
        #region Settings Region
        public void Pause(PauseLevel pauseLevel)
        {
            switch (pauseLevel)
            {
                case PauseLevel.Dialog: DisableAllGroups(); break;
                case PauseLevel.UI: EnableOnlyGroup(settingsGroup); break;
            }
        }
        public void Unpause()
        {
            EnableOnlyGroup(mapGroup);
            audioGallery.PlayMusic(musicToPlay);
        }
        #endregion
    }
}