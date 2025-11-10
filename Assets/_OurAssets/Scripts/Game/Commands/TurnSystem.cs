
using CursedOnion.Game.Commands;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Modes.General.UI.Transitions;
using CursedOnion.Game.Systems.Level;
using CursedOnion.ScriptableObjects;
using NaughtyAttributes;
using Reflex.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CursedOnion
{
    public class TurnSystem : MonoBehaviour
    {
        [Inject] private LevelAsset levelAsset;
        [Inject] private CommandManager commandManager;
        
        [BoxGroup("End Game"), Scene, SerializeField] private string resetScene;
        [BoxGroup("End Game"), SerializeField] UITransitionData transitionData;
        
        int currentInitiative;
        
        [SerializeField]private List<Unit> allies = new List<Unit>();
        [SerializeField] private List<Unit> enemies = new List<Unit>();

        [SerializeField] private List<Unit> activeUnits = new List<Unit>();
        public List<Unit> GetActiveUnits() => activeUnits;

        public event Action<Unit> OnUnitTurnStart;
        public event Action<Unit> OnUnitTurnEnd;

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
            Debug.Log("======== NUEVA RONDA EMPIEZA ========");
            
            if (allies.Count == 0 || enemies.Count == 0) return;
            
            allies = allies.OrderByDescending(u => u.GetStats().InitiativeStat).ToList();
            enemies = enemies.OrderByDescending(u => u.GetStats().InitiativeStat).ToList();

            var maxAllyInitiative = allies[0].GetStats().InitiativeStat;
            var maxEnemyInitiative = enemies[0].GetStats().InitiativeStat;
            currentInitiative = Mathf.Max(maxEnemyInitiative, maxAllyInitiative);
            
            //NextTurn();
            StartInitiativeGroup();
        }

        private void StartInitiativeGroup()
        {
            Debug.Log($"-- Iniciativa actual: {currentInitiative} --");
            commandManager.ClearStack();

            // Buscar todas las unidades que tengan la iniciativa actual
            var allyGroup = allies.Where(u => u.GetStats().InitiativeStat == currentInitiative).ToList();
            var enemyGroup = enemies.Where(u => u.GetStats().InitiativeStat == currentInitiative).ToList();

            activeUnits.Clear();
            activeUnits.AddRange(allyGroup);
            activeUnits.AddRange(enemyGroup);

            if (activeUnits.Count == 0)
            {
                // Si no hay unidades en esta iniciativa, pasa a la siguiente
                currentInitiative--;
                if (currentInitiative > 0)
                    StartInitiativeGroup();
                else
                    StartRound(); // Reinicia cuando termina todo el ciclo
                return;
            }

            // Si hay aliados en esta iniciativa, el jugador elegirá el orden
            if (allyGroup.Count > 0)
            {
                Debug.Log($"Turno del jugador unidades disponibles: {allyGroup.Count}");
                foreach (var unit in allyGroup)
                {
                    OnUnitTurnStart?.Invoke(unit); // Notifica que empieza el turno de esta unidad
                    unit.UnitController.ProcessTurn(unit);
                }
            }
            else
            {
                // Turno del enemigo (IA)
                Debug.Log($"Turno de la IA (Iniciativa {currentInitiative})");
                foreach (var unit in enemyGroup)
                {
                    OnUnitTurnStart?.Invoke(unit);
                    unit.UnitController.ProcessTurn(unit);
                }
            }
        }

        public void EndTurnForUnit(Unit unit)
        {
            if (activeUnits.Contains(unit))
            {
                activeUnits.Remove(unit);
                OnUnitTurnEnd?.Invoke(unit); // Notifica que terminó el turno
            }

            // Si todas las unidades con esta iniciativa terminaron
            if (activeUnits.Count == 0)
            {
                currentInitiative--;
                if (currentInitiative > 0)
                    StartInitiativeGroup();
                else
                    StartRound();
            }
        }

        public void EndTurn()
        {
            activeUnits.Clear();
            currentInitiative--;
            if (currentInitiative > 0)
                StartInitiativeGroup();
            else
                StartRound();
        }

    }
}
