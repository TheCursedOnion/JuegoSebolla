using CursedOnion.Game.Commands;
using CursedOnion.Game.Events;
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
            associatedUnit = unit;
            associatedUnit.OnEntityUpdate -= UpdateUI;
            associatedUnit.OnEntityUpdate += UpdateUI;
            UpdateUI(associatedUnit);
        }

        void UpdateUI(SimpleEntity entity)
        {
            if(entity is not Unit unit) return;
            
            var flags = unit.GetFlags();
            
            moveButton.SetInteractive(!flags.HasMoved());
            attackButton.SetInteractive(!flags.HasAttacked());
            specialButton.SetInteractive(!flags.HasUsedAbility());
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
