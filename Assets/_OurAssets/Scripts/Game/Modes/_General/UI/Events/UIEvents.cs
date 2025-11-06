using System;
using CursedOnion.Game.General.UI.Buttons;
using UnityEngine;

namespace CursedOnion.Game.Modes.General.UI.Events
{
    public class UIEvents : MonoBehaviour
    {
        public event Action<UIButton> OnButtonSelected;
        public event Action<int> OnButtonGroupSelected;
        
        public void SelectButton(UIButton button)
        {
            OnButtonSelected?.Invoke(button);
        }
        public void SelectButtonGroup(int group)
        {
            OnButtonGroupSelected?.Invoke(group);
        }
        
    }
}
