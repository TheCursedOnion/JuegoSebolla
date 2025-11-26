using CursedOnion.Game.Commands;
using CursedOnion.Game.General.UI.Buttons;
using CursedOnion.Game.Modes.General.UI.Events;
using CursedOnion.Game.Systems.Level;
using Reflex.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace CursedOnion.Game.Entity.UI    
{
    public class UnitUI : MonoBehaviour
    {
        [SerializeField] private UIButton moveButton;
        [SerializeField] private UIButton attackButton;
        [SerializeField] private UIButton specialButton;
        [SerializeField] private Image abilityImage;
        
        LevelEvents levelEvents;
        UIEvents uiEvents;
        Unit associatedUnit;
        CommandParameters commonParameters;
        public void Initialize()
        {
            var container = gameObject.scene.GetSceneContainer();
            levelEvents = container.Resolve<LevelEvents>();
            uiEvents = container.Resolve<UIEvents>();
            
            CommandParameters.Builder parametersBuilder = new CommandParameters.Builder();
            commonParameters = parametersBuilder.SetExecuteOnce(true).Build();
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
            moveButton.SetInteractive(!actions.HasMoved() && !isNotIdle);
            attackButton.SetInteractive(!actions.HasAttacked() && !isNotIdle);
            specialButton.SetInteractive(!actions.HasUsedAbility() && !isNotIdle);
            abilityImage.sprite = associatedUnit.StatData.SpecialAbility?.AbilityIcon;
        }

        public void SelectButtonUIEvent(UIButton button)
        {
            uiEvents.SelectButton(button);
        }
        public void MoveUnit()
        {
            levelEvents.CallPrepareCommand<MoveCommand>(commonParameters);
        }

        public void AttackUnit()
        {
            levelEvents.CallPrepareCommand<AttackCommand>(commonParameters);
        }

        public void AbilityActivation()
        {
            levelEvents.CallPrepareCommand<AbilityCommand>(commonParameters);
        }
    }
}
