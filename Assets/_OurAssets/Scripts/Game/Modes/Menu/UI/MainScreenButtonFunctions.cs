using CursedOnion.Game.General.UI.Buttons;
using CursedOnion.Game.Modes.General.UI.Events;
using UnityEngine;

namespace CursedOnion.Game.Modes.Menu.UI
{
    public class MainScreenButtonFunctions : MonoBehaviour
    {
        [SerializeField] private UIButton playButton;
        [SerializeField] private UIButton optionsButton;
        [SerializeField] private UIButton creditsButton;
        
        public void DisableButtons()
        {
            playButton.SetInterative(false);
            optionsButton.SetInterative(false);
            creditsButton.SetInterative(false);
        }
    }
}
