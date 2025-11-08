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
        [SerializeField] private Button moveButton;
        [SerializeField] private Button attackButton;
        [SerializeField] private Button abilityButton;
        public void Initialize()
        {
            levelEvents = gameObject.scene.GetSceneContainer().Resolve<LevelEvents>();
        }

        public void AssociateUnit(Unit unit)
        {
            associatedUnit = unit;
            associatedUnit.OnEntityUpdate -= UpdateUI;
            associatedUnit.OnEntityUpdate += UpdateUI;
            UpdateUI();
        }

        void UpdateUI()
        {
            Debug.Log("Update UI");
        }
        
        public void MoveUnit()
        {
            levelEvents.CallPrepareCommand<MoveCommand>();
        }

        public void AttackUnit()
        {
            levelEvents.CallPrepareCommand<AttackCommand>();
        }

        public void AbilityActivation()
        {
            levelEvents.CallPrepareCommand<AbilityCommand>();
        }
    }
}
