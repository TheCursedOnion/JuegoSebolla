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
            SetInteractive(Interactable);
            RegisterEvents();
        }
        protected void OnDestroy()
        {
            UnRegisterEvents();
        }
        protected virtual void RegisterEvents()
        {
            UiEvents.OnButtonSelected += OnSelectButton;
            UiEvents.OnButtonUnselected += OnUnselectButton;
            
            UiEvents.OnButtonGroupSelected += OnSelectButtonGroup;
        }
        protected virtual void UnRegisterEvents()
        {
            UiEvents.OnButtonSelected -= OnSelectButton;
            UiEvents.OnButtonUnselected -= OnUnselectButton;
            
            UiEvents.OnButtonGroupSelected -= OnSelectButtonGroup;
        }
        protected void OnSelectButton(UIButton button)
        {
            SelectButton(button == this);
        }
        protected void OnUnselectButton(UIButton button)
        {
            if(button == this) SelectButton(false);
        }
        protected void OnSelectButtonGroup(int group)
        {
            SelectButton(group == ButtonGroupId);
        }
        protected void SelectButton(bool select)
        {
            if(IsSelected == select) return;
            
            IsSelected = select;
            InvokeSelectionEvents();
        }
        protected void InvokeSelectionEvents()
        {
            if(IsSelected) OnSelect.Invoke();
            else OnDeselect.Invoke();
        }
        
        
        public void ToggleSelfSelect()
        {
            if(!IsSelected)
                UiEvents.SelectButton(this);
            else
                UiEvents.UnselectButton(this);
        }
        public void SelfSelect()
        {
            UiEvents.SelectButton(this);
        }
        
        public override void SetInteractive(bool isInteractive)
        {
            base.SetInteractive(isInteractive);
            
            if(!isInteractive) SelectButton(false);
            
            UnityButton ??= GetComponent<Button>();
            UnityButton.interactable = Interactable;
        }
        
    }
}
