
using CursedOnion.ScriptableObjects;
using Reflex.Attributes;

using System.Collections.Generic;
using System.Linq;
using CursedOnion.Game.Commands;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Systems.Level;
using CursedOnion.Game.Modes.General.UI.Transitions;
using NaughtyAttributes;
using UnityEngine;

namespace CursedOnion
{
    public class TurnSystem : MonoBehaviour
    {
        [Inject] private LevelAsset levelAsset;
        [Inject] private CommandManager commandManager;
        
        [BoxGroup("End Game"), Scene, SerializeField] private string resetScene;
        [BoxGroup("End Game"), SerializeField] UITransitionData transitionData;
        
        int currentInitiative = 0;
        
        private List<Unit> allies = new List<Unit>();
        private List<Unit> enemies = new List<Unit>();
        
        private List<Unit> activeUnits;

        public void AddUnit(Unit unit)
        {
            if (unit.Side == BattleSide.Enemy)
            {
                if(!enemies.Contains(unit)) enemies.Add(unit);
            }
            else if (unit.Side == BattleSide.Ally)
            {
                if (!allies.Contains(unit)) allies.Add(unit);
            }
        }
        public void RemoveUnit(Unit unit)
        {
            if(allies.Contains(unit)) allies.Remove(unit);
            if(enemies.Contains(unit)) enemies.Remove(unit);
        }

        public void StartRound()
        { 
            if(allies.Count == 0 || enemies.Count == 0) return;
            
            allies = allies.OrderByDescending(u => u.GetStats().InitiativeStat).ToList();
            enemies = enemies.OrderByDescending(u => u.GetStats().InitiativeStat).ToList();

            var maxAllyInitiative = allies[0].GetStats().InitiativeStat;
            var maxEnemyInitiative = enemies[0].GetStats().InitiativeStat;
            currentInitiative = Mathf.Max(maxEnemyInitiative, maxAllyInitiative) + 1;
            
            NextTurn();
        }

        private bool CallPlayerTurn()
        {
            return CallTurn(false);
        }
        private bool CallEnemyTurn()
        {
            return CallTurn(true);
        }
        private bool CallTurn(bool forPlayer)
        {
            if (forPlayer)
            {
                activeUnits = allies.Where(u=> currentInitiative == u.GetStats().InitiativeStat).ToList();
            }
            else
            {
                activeUnits = enemies.Where(u=> currentInitiative == u.GetStats().InitiativeStat).ToList();
            }

            foreach (var unit in activeUnits)
            {
                unit.UnitController.ProcessTurn();
            }
            
            bool result = activeUnits.Count > 0;
            return result;
        }


        public void EndTurnForUnit(Unit unit)
        {
            if (activeUnits.Contains(unit)) activeUnits.Remove(unit);

            if (activeUnits.Count == 0)
            {
                NextTurn();
            }
        }

        public void EndTurn()
        {
            activeUnits.Clear();
            NextTurn();
        }

        void NextTurn()
        {
            commandManager.ClearStack();
            
            bool hasActiveUnits = false;
            while (!hasActiveUnits)
            {
                UpdateIniciative();
                
                hasActiveUnits = CallPlayerTurn();
                if (!hasActiveUnits) CallEnemyTurn();
                
                if (currentInitiative == 0) break;
            }
            
            if(!hasActiveUnits && allies.Count > 0 && enemies.Count > 0) StartRound();
        }
        
        void UpdateIniciative()
        {
            currentInitiative--;
        }


    }
}
