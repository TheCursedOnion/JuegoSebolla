using BehaviourAPI.Core;
using BehaviourAPI.UnityToolkit.GUIDesigner.Runtime;
using CursedOnion.Game.Entity.UI;
using CursedOnion.Game.Systems.Level;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Entity
{
    public class AIUnitController : EntityComponentController
    {
        [Inject] LevelManager levelManager;
        AssetBehaviourRunner runner;
        public bool startTurn;

        public AssetBehaviourRunner GetBehaviourRunner() => runner;

        TurnSystem turnSystem;

        public void Start()
        {
            runner = gameObject.GetComponent<AssetBehaviourRunner>();
            turnSystem = levelManager.GetTurnSystem();
        }

        public override void ProcessTurn()
        {
            startTurn = true;
        }

        public bool StartTurn()
        {
            return startTurn;
        }

        public bool IsEnemyClose()
        {
            return true;
        }

        public void HaEntrado()
        {
            Debug.Log("HA ENTRADO");

        }

        public void Salchipapa()
        {
            Debug.Log("Salchipapa!");
        }

        public Status EndAction()
        {
            Debug.Log("TERMINA HOSTIAS");

            return Status.Success;
        }

        public void EndAITurn()
        {
            var unit = gameObject.GetComponent<Unit>();

            if (turnSystem.GetActiveUnits().Contains(unit))
            {
                startTurn = false;
                turnSystem.EndTurnForAIUnit(unit);
            }
        }
    }
}
