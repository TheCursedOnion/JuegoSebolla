using CursedOnion.Game.Commands;
using CursedOnion.Game.Handlers;
using Reflex.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace CursedOnion.Game.Entity.UI    
{
    public class UnitUI : MonoBehaviour
    {
        EntityCommandHandler commandHandler;
        Unit associatedUnit;
        [SerializeField] private Button moveButton;
        [SerializeField] private Button attackButton;
        public void Initialize()
        {
            commandHandler = gameObject.scene.GetSceneContainer().Resolve<LevelManager>().CommandHandler;
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
            commandHandler.PrepareEntityCommand<MoveCommand>();
        }

        public void AttackUnit()
        {
            commandHandler.PrepareEntityCommand<AttackCommand>();
        }
    }
}
