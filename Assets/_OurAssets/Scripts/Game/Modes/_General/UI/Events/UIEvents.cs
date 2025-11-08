using System;
using CursedOnion.Game.General.UI.Buttons;
using UnityEngine;

namespace CursedOnion.Game.Modes.General.UI.Events
{
    public class UIEvents
    {
        public event Action<UIButton> OnButtonSelected;
        public event Action<int> OnButtonGroupSelected;
        
        public void SelectButton(UIButton interactiveButton)
        {
            OnButtonSelected?.Invoke(interactiveButton);
        }
        public void SelectButtonGroup(int group)
        {
            OnButtonGroupSelected?.Invoke(group);
        }
        
    }
}
