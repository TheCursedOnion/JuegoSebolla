
using CursedOnion.ScriptableObjects;
using Reflex.Attributes;

using System.Collections.Generic;
using System.Linq;
using CursedOnion.Game.Commands;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Systems.Level;
using CursedOnion.UI.Transitions;
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
            if (unit.IsEnemy)
            {
                if(!enemies.Contains(unit)) enemies.Add(unit);
            }
            else
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

        /*private void TryToMoveCharacter()
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("Click sobre UI: ignorando intento de ataque.");
                return;
            }
            Debug.Log("Trying to move character...");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var levelGrid = levelAsset.Grid;
                Vector3 hitPoint = hit.point - hit.normal * 0.1f;

                if (levelGrid.TryWorldToGridPosition(hitPoint, out Vector3 gridPosition))
                {
                    Tile3d aboveTile = levelGrid.GetTileAtGridPosition(gridPosition + Vector3.up);

                    if (aboveTile != null && aboveTile.GetContainedEntity() == null)
                    {
                        if (levelGrid.TryGridToWorldPosition(gridPosition, out Vector3 worldPosition))
                        {
                            Debug.Log("Moving to grid position: " + gridPosition + " at world position: " + worldPosition.Center());
                        }
                        else
                        {
                            Debug.Log("Failed to convert grid position to world position: " + gridPosition + worldPosition + levelGrid.StartingOffset);
                            return;
                        }

                        var moveCmd = EntityCommand.Create<MoveCommand>(units[currentCharacterIndex], worldPosition.Center() + new Vector3(0f, 1.0f, 0f), levelGrid);
                        commandManager.ExecuteCommand(moveCmd);
                    }
                    else
                    {
                        units[currentCharacterIndex].Flags.CanMove = false;
                        Debug.Log("Not valid tile: " + gridPosition);
                    }
                }
            }
        }

        private void TryToAttackCharacter()
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("Click sobre UI: ignorando intento de ataque.");
                return;
            }
            Debug.Log("Trying to attack character...");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var levelGrid = levelAsset.Grid;
                Vector3 hitPoint = hit.point - hit.normal * 0.1f;

                if (levelGrid.TryWorldToGridPosition(hitPoint, out Vector3 gridPosition))
                {
                    Tile3d aboveTile = levelGrid.GetTileAtGridPosition(gridPosition + Vector3.up);
                    
                    if (aboveTile != null && aboveTile.GetContainedEntity() != null)
                    {
                        Unit targetChar = aboveTile.GetContainedEntity() as Unit;
                        Unit attacker = units[currentCharacterIndex];

                        if (targetChar != null && targetChar != attacker)
                        {
                            if (attacker.IsEnemy != targetChar.IsEnemy)
                            {
                                var attackCmd = EntityCommand.Create<AttackCommand>(attacker, targetChar, levelGrid);
                                commandManager.ExecuteCommand(attackCmd);
                            }
                            else
                            {
                                units[currentCharacterIndex].Flags.CanAttack = false;
                                Debug.Log("Same team attacking. Not valid Action.");
                            }
                        }
                        else
                        {
                            units[currentCharacterIndex].Flags.CanAttack = false;
                            Debug.Log("Not valid target: " + gridPosition);
                        }
                    }
                    else
                    {
                        units[currentCharacterIndex].Flags.CanAttack = false;
                        Debug.Log("Not valid tile: " + gridPosition);
                    }
                }
            }
        }*/

        /*public void HandleEntitySelection(IEntity entity)
        {
            if (entity == null) return;
            if (orderedCharacters == null || orderedCharacters.Count == 0) return;

            var selectedCharacter = entity as Character;

            foreach (var c in characters)
            {
                c.uiScript?.HideUI();
            }

            var current = orderedCharacters[currentCharacterIndex];
            bool isCurrentTurn = selectedCharacter.id == current.id;

            if (selectedCharacter.uiScript != null)
            {
                selectedCharacter.uiScript.ShowForSelection(isCurrentTurn);
            }

        }*/

        /*private void HandleCharacterDied(Character c)
        {
            Debug.Log($"TurnSystem: personaje muerto -> {c.characterName} (id {c.id}). Comprobando fin de juego...");
            CheckForEndGame();
        }

        private void CheckForEndGame()
        {
            if (gameEnded) return;

            int aliveAllies = characters.Count(ch => !ch.isEnemy && !ch.hasDied);
            int aliveEnemies = characters.Count(ch => ch.isEnemy && !ch.hasDied);

            if (aliveAllies == 0 && aliveEnemies == 0)
            {
                EndGame(null); 
            }
            else if (aliveAllies == 0)
            {
                EndGame(true); 
            }
            else if (aliveEnemies == 0)
            {
                EndGame(false); 
            }
        }

        private void EndGame(bool? enemyWon)
        {
            gameEnded = true;
            SceneServiceUser sceneServiceUser = GetComponent<SceneServiceUser>();
            Color transitionColor;
            string endMessage;
            if (enemyWon == true)
            {
                endMessage = "EndGame: Los ENEMIGOS han ganado.";
                transitionColor = Color.red;
            }
            else if (enemyWon == false)
            {
                endMessage = "EndGame: Los ALIADOS han ganado.";
                transitionColor = Color.green;
            }
            else
            {
                endMessage = "EndGame: Empate, no quedan unidades.";
                transitionColor = Color.gray;
            }
            
            Debug.Log(endMessage);
            transitionData.Color = transitionColor;
            sceneServiceUser.ChangeScene(resetScene, transitionData);
        }*/


    }
}
