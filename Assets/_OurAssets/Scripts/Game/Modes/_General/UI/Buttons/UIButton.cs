using System;
using CursedOnion.Game.Modes.General.UI.Events;
using NaughtyAttributes;
using Reflex.Attributes;
using Reflex.Extensions;
using UltEvents;
using UnityEngine;
using UnityEngine.UI;

namespace CursedOnion.Game.General.UI.Buttons
{
    public class UIButton : UIInteractiveElement
    {
        [Inject] protected UIEvents UiEvents;
        protected Button UnityButton;
        protected bool IsSelected;
        
        [HorizontalLine(height: 2f, color: EColor.Red)]
        [SerializeField] protected int ButtonGroupId;
        [SerializeField] protected UltEvent OnSelect;
        [SerializeField] protected UltEvent OnDeselect;
        protected void Awake()
        {
            UiEvents ??= gameObject.scene.GetSceneContainer().Resolve<UIEvents>();
            UnityButton = GetComponent<Button>();
        }
        protected virtual void OnEnable()
        {
            UiEvents.OnButtonSelected += OnSelectButton;
            UiEvents.OnButtonGroupSelected += OnSelectButtonGroup;
        }
        protected virtual void OnDisable()
        {
            UiEvents.OnButtonSelected -= OnSelectButton;
            UiEvents.OnButtonGroupSelected -= OnSelectButtonGroup;
        }
        protected void OnSelectButton(UIButton button)
        {
            SelectButton(button == this);
        }
        protected void OnSelectButtonGroup(int group)
        {
            SelectButton(group == ButtonGroupId);
        }
        protected void SelectButton(bool select)
        {
            if(select == IsSelected) return;
            IsSelected = select;
            InvokeSelectionEvents();
        }
        protected void InvokeSelectionEvents()
        {
            if(IsSelected) OnSelect.Invoke();
            else OnDeselect.Invoke();
        }
        
        public void SetInteractive(bool isInteractive)
        {
            UnityButton ??= GetComponent<Button>();
            
            if(!isInteractive) SelectButton(false);
            
            Interactable = isInteractive;
            UnityButton.interactable = Interactable;
        }
        
    }
}
