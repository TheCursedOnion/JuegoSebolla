using CursedOnion.Game.General.UI.Buttons;
using CursedOnion.Locators;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Modes.Map.UI
{
    public class TimePeriodPanel : MonoBehaviour
    {
        [Inject] RuntimeVariableLocator variableLocator;
        
        [SerializeField] UIButton[] timePeriodButtons;
        void Awake()
        {
            int lastCompletedWorld = (variableLocator.LastCompletedLevel + 1) / 4;
            for (int i = 0; i < timePeriodButtons.Length; i++)
            {
                timePeriodButtons[i].SetInteractive(i <= lastCompletedWorld);
            }
        }
    }
}
