using System;
using CursedOnion.Game.General.UI.Buttons;
using UnityEngine;

namespace CursedOnion.Game.Modes.General.UI.Events
{
    public class UIEvents
    {
        public event Action<UIButton> OnButtonSelected;
        public event Action<UIButton> OnButtonUnselected;
        
        public void SelectButton(UIButton interactiveButton)
        {
            OnButtonSelected?.Invoke(interactiveButton);
        }
        public void UnselectButton(UIButton interactiveButton)
        {
            OnButtonUnselected?.Invoke(interactiveButton);
        }
        
        public event Action<int> OnButtonGroupSelected;
        public event Action<int> OnButtonGroupUnselected;
        public void SelectButtonGroup(int group)
        {
            OnButtonGroupSelected?.Invoke(group);
        }
        public void UnselectButtonGroup(int group)
        {
            OnButtonGroupUnselected?.Invoke(group);
        }
        
        public void UnselectAllButtons()
        {
            OnButtonSelected?.Invoke(null);
        }
        
    }
}
