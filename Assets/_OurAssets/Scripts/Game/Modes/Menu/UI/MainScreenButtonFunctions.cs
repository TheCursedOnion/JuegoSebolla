using CursedOnion.Game.General.UI.Buttons;
using CursedOnion.Game.Modes.General.UI.Events;
using CursedOnion.Game.Settings;
using Reflex.Extensions;
using UnityEngine;

namespace CursedOnion.Game.Modes.Menu.UI
{
    public class MainScreenButtonFunctions : MonoBehaviour
    {
        [SerializeField] private UIButton playButton;
        [SerializeField] private UIButton optionsButton;
        [SerializeField] private UIButton creditsButton;
        [SerializeField] private UIButton exitButton;
        
        GameSettings gameSettings;
        public void DisableButtons()
        {
            playButton.SetInteractive(false);
            optionsButton.SetInteractive(false);
            creditsButton.SetInteractive(false);
            exitButton.SetInteractive(false);
        }
        public void ExitGame() => Application.Quit();

        public void SaveSettings()
        {
            gameSettings ??= gameObject.scene.GetSceneContainer().Resolve<GameSettings>();
            _ = gameSettings.Save();
        }
    }
}
