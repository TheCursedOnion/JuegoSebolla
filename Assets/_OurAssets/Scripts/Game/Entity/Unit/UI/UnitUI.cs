using CursedOnion.Game.Commands;
using CursedOnion.Game.Events;
using CursedOnion.Game.Systems.Level;
using Reflex.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace CursedOnion.Game.Entity.UI    
{
    public class UnitUI : MonoBehaviour
    {
        LevelEvents levelEvents;
        
        Unit associatedUnit;
        
        CommandParameters commonParameters;
        public void Initialize()
        {
            levelEvents = gameObject.scene.GetSceneContainer().Resolve<LevelEvents>();

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
            
            //Debug.Log("Update UI");
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
