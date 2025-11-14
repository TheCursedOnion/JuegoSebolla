using BehaviourAPI.Core;
using BehaviourAPI.UnityToolkit.GUIDesigner.Runtime;
using CursedOnion.Game.Entity.UI;
using CursedOnion.Game.Systems.Level;
using JetBrains.Annotations;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Entity
{
    public class AIUnitController : EntityComponentController
    {
        [Inject] LevelManager levelManager;
        AssetBehaviourRunner runner;
        bool startTurn;

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

        public bool IsEnemyInAttackRange()
        {
            return true;
        }

        public void EnemyAttack()
        {
            Debug.Log("EL ENEMIGO VA A ATACAR");

        }
        public Status EndAttack()
        {
            Debug.Log("ENEMY HA ATACDO: SUCCESS");

            return Status.Success;
        }


        public bool IsEnemyInMovementRange()
        {
            return true;
        }

        public void EnemyMove()
        {
            Debug.Log("EL ENEMIGO VA A MOVERSE");

        }

        public void SearchAndMoveToUnit()
        {
            Debug.Log("EL ENEMIGO VA A BUSCAR UNA UNIDAD Y MOVERSE HACIA ELLA");
        }

        public Status EndMove()
        {
            Debug.Log("ENEMY SE HA MOVIDO: SUCCESS");

            return Status.Success;
        }

        public void EndAITurn()
        {
            if (turnSystem != null && gameObject.GetComponent<Unit>() != null)
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
}
