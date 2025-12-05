using System;
using CursedOnion.Game.Commands;
using CursedOnion.Game.General.UI.Buttons;
using CursedOnion.Game.Modes.General.UI.Events;
using CursedOnion.Game.Systems.Level;
using Reflex.Attributes;
using Reflex.Extensions;
using Reflex.Injectors;
using UnityEngine;
using UnityEngine.UI;

namespace CursedOnion.Game.Entity.UI    
{
    public class UnitUI : MonoBehaviour
    {
        enum CommandMode
        {
            None = -1,
            Move = 0,
            Attack = 1,
            Ability = 2
        }

        [SerializeField] private UIButton undoMoveButton;
        [SerializeField] private UIButton moveButton;
        [SerializeField] private UIButton attackButton;
        [SerializeField] private UIButton specialButton;
        [SerializeField] private Image abilityImage;
        
        [Inject] LevelEvents levelEvents;
        [Inject] UIEvents uiEvents;
        [Inject] CommandManager commandManager;
        
        Unit associatedUnit;
        CommandParameters commonParameters;
        CommandMode lastMode = CommandMode.None;
        UIButton lastSelectedButton;
        public void Initialize()
        {
            var container = gameObject.scene.GetSceneContainer();
            AttributeInjector.Inject(this, container);
            
            CommandParameters.Builder parametersBuilder = new CommandParameters.Builder();
            commonParameters = parametersBuilder.SetExecuteOnce(true).Build();

            levelEvents.OnPreparedCommandLaunched += UpdateLastMode;
        }
        void OnDestroy()
        {
            if (associatedUnit != null) associatedUnit.OnEntityUpdate -= ProcessEntityUpdate;
            levelEvents.OnPreparedCommandLaunched -= UpdateLastMode;
        }
        public void AssociateUnit(Unit unit)
        {
            if (associatedUnit != null) associatedUnit.OnEntityUpdate -= ProcessEntityUpdate;
            
            associatedUnit = unit;
            associatedUnit.OnEntityUpdate += ProcessEntityUpdate;
            
            UpdateUI();
        }
        void ProcessEntityUpdate(SimpleEntity entity)
        {
            if (entity is Unit && entity == associatedUnit) UpdateUI();
        }
        void UpdateUI()
        {
            var actions = associatedUnit.ActionHandler;
            bool isNotIdle = actions.IsNotIdle();
            lastMode = CommandMode.None;
            
            undoMoveButton.SetInteractive(actions.HasMoved() && !isNotIdle && commandManager.HasCommandsStacked());
            moveButton.SetInteractive(!actions.HasMoved() && !isNotIdle);
            attackButton.SetInteractive(!actions.HasAttacked() && !isNotIdle);
            specialButton.SetInteractive(!actions.HasUsedAbility() && !isNotIdle);
            abilityImage.sprite = associatedUnit.StatData.SpecialAbility?.AbilityIcon;
        }
        
        void UpdateLastMode(bool _, Type __)
        {
            lastMode = CommandMode.None;
            uiEvents.UnselectAllButtons();
        }

        public void UndoMove()
        {
            commandManager.Undo();
            levelEvents.CancelPreparedCommand();
            uiEvents.UnselectAllButtons();
            lastMode = CommandMode.None;
        }
        public void MoveUnit(UIButton button)
        {
            if (lastMode != CommandMode.Move)
            {
                levelEvents.CallPrepareCommand<MoveCommand>(commonParameters);
                uiEvents.SelectButton(button);
                lastMode = CommandMode.Move;
            }
            else
            {
                levelEvents.CancelPreparedCommand();
                uiEvents.UnselectButton(button);
                lastMode = CommandMode.None;
            }
        }

        public void AttackUnit(UIButton button)
        {
            if (lastMode != CommandMode.Attack)
            {
                levelEvents.CallPrepareCommand<AttackCommand>(commonParameters);
                uiEvents.SelectButton(button);
                lastMode = CommandMode.Attack;
            }
            else
            {
                levelEvents.CancelPreparedCommand();
                uiEvents.UnselectButton(button);
                lastMode = CommandMode.None;
            }
        }

        public void AbilityActivation(UIButton button)
        {
            if (lastMode != CommandMode.Ability)
            {
                levelEvents.CallPrepareCommand<AbilityCommand>(commonParameters);
                levelEvents.SelectSpecialAbility(associatedUnit);
                uiEvents.SelectButton(button);
                lastMode = CommandMode.Ability;
            }
            else
            {
                levelEvents.CancelPreparedCommand();
                uiEvents.UnselectButton(button);
                lastMode = CommandMode.None;
            }
        }
    }
}
