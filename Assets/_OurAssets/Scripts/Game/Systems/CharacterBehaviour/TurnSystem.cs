using CursedOnion.Extensions;
using CursedOnion.Game;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.ScriptableObjects;
using Reflex.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using CursedOnion.Game.Logic.Services;
using CursedOnion.UI.Transitions;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using static UnityEngine.GraphicsBuffer;

namespace CursedOnion
{
    public class TurnSystem : MonoBehaviour
    {
        [Inject] private CommandManager commandManager;
        [Inject] private LevelAsset levelAsset;
        [Inject] private LevelManager levelManager;

        [SerializeField] private CharacterData[] characterTypes;

        [BoxGroup("End Game"), Scene, SerializeField] private string resetScene;
        [BoxGroup("End Game"), SerializeField] UITransitionData transitionData;

        private List<Character> characters = new List<Character>();
        private List<Character> orderedCharacters = new List<Character>();

        private int currentCharacterIndex = 0;
        private int turnCount = 1;
        private bool waitingForInput = true;
        private int spawned = 0;
        public bool canSpawnUnit = false;

        private bool gameEnded = false;

        public event Action OnSpawnPhaseEnded;

        void Start()
        {
            Debug.Log("Placing units phase starts!");
            canSpawnUnit = false;
            Character.OnCharacterDied += HandleCharacterDied;
        }

        void OnDestroy()
        {
            Character.OnCharacterDied -= HandleCharacterDied;
        }

        private void StartTurn()
        {
            if (characters.Count > 0)
            {
                canSpawnUnit = false;
                Debug.Log("Termina la fase de colocaci�n de unidades");
                OnSpawnPhaseEnded?.Invoke();

                Debug.Log("Turno " + turnCount + " comienza.");
                orderedCharacters[currentCharacterIndex].DoTurn();
            }
        }

        void Update()
        {
            if (gameEnded) return;

            if (waitingForInput && Input.GetKeyDown(KeyCode.Space))
            {
                orderedCharacters[currentCharacterIndex].EndTurn();
                foreach (var c in characters)
                {
                    c.uiScript?.HideUI();
                }
                NextTurn();
            }
            if (waitingForInput && Input.GetKeyDown(KeyCode.Return))
            {
                if (characters.Count == 0)
                {
                    Debug.Log("Theres NO Characters");
                }
                else
                {
                    PrintStats();
                    StartTurn();
                }
            }
            if (waitingForInput && canSpawnUnit && Input.GetMouseButtonDown(0))
            {
                TryToSpawnCharacter();
            }

            if (orderedCharacters.Count > 0)
            {
                if (waitingForInput && orderedCharacters[currentCharacterIndex].canAttack && Input.GetMouseButtonDown(0))
                {
                    TryToAttackCharacter();
                }
                if (waitingForInput && orderedCharacters[currentCharacterIndex].canMove && Input.GetMouseButtonDown(0))
                {
                    TryToMoveCharacter();
                }
                if (waitingForInput && Input.GetKeyDown(KeyCode.U))
                {
                    commandManager.Undo();
                }
                if (waitingForInput && Input.GetKeyDown(KeyCode.R))
                {
                    commandManager.Redo();
                }
            }
        }

        void NextTurn()
        {
            if (gameEnded) return;
            do
            {
                orderedCharacters[currentCharacterIndex].uiScript.gameObject.SetActive(false);
                currentCharacterIndex++;
                if (currentCharacterIndex >= orderedCharacters.Count)
                {
                    currentCharacterIndex = 0;
                    turnCount++;
                    Debug.Log("Turno " + turnCount + " comienza.");
                }
            }
            while (orderedCharacters[currentCharacterIndex].hasDied);

            commandManager.ClearTurn();
            orderedCharacters[currentCharacterIndex].DoTurn();
        }

        void TryToSpawnCharacter()
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("Click sobre UI: ignorando intento de ataque.");
                return;
            }
            Debug.Log("Trying to Spawn character...");
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
                            Debug.Log("Spawning Cahracter in grid position: " + gridPosition + " at world position: " + worldPosition.Center());
                        }
                        else
                        {
                            Debug.Log("Failed to convert grid position to world position: " + gridPosition + worldPosition + levelGrid.StartingOffset);
                            return;
                        }

                        SpawnRandomCharacter(worldPosition, aboveTile);
                    }
                    else
                    {
                        Debug.Log("Not valid tile: " + gridPosition);
                    }
                }
            }
        }

        private void TryToMoveCharacter()
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

                        var moveCmd = CharacterCommand.Create<MoveCommand>(orderedCharacters[currentCharacterIndex], worldPosition.Center() + new Vector3(0f, 1.0f, 0f), levelGrid);
                        commandManager.ExecuteCommand(moveCmd);
                    }
                    else
                    {
                        orderedCharacters[currentCharacterIndex].canMove = false;
                        orderedCharacters[currentCharacterIndex].UpdateCharacterUI();
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
                        Character targetChar = aboveTile.GetContainedEntity() as Character;
                        Character attacker = orderedCharacters[currentCharacterIndex];

                        if (targetChar != null && targetChar.id != attacker.id)
                        {

                            if (attacker.isEnemy != targetChar.isEnemy)
                            {
                                var attackCmd = CharacterCommand.Create<AttackCommand>(attacker, targetChar, levelGrid);
                                commandManager.ExecuteCommand(attackCmd);
                            }
                            else
                            {
                                orderedCharacters[currentCharacterIndex].canAttack = false;
                                orderedCharacters[currentCharacterIndex].UpdateCharacterUI();
                                Debug.Log("Same team attacking. Not valid Action.");
                            }
                        }
                        else
                        {
                            orderedCharacters[currentCharacterIndex].canAttack = false;
                            orderedCharacters[currentCharacterIndex].UpdateCharacterUI();
                            Debug.Log("Not valid target: " + gridPosition);
                        }
                    }
                    else
                    {
                        orderedCharacters[currentCharacterIndex].canAttack = false;
                        orderedCharacters[currentCharacterIndex].UpdateCharacterUI();
                        Debug.Log("Not valid tile: " + gridPosition);
                    }
                }
            }
        }
        void SpawnRandomCharacter(Vector3 worldPosition, Tile3d tile)
        {
            GameObject charObj = new GameObject("Character_" + spawned);

            charObj.transform.position = worldPosition.Center() + new Vector3(0f, 1.0f, 0f);

            Character character = charObj.AddComponent<Character>();
            CharacterData randomType = characterTypes[UnityEngine.Random.Range(0, characterTypes.Length)];

            character.data = randomType;
            character.id = spawned;
            character.SetCharacterData();

            GameObject modelInstance = Instantiate(character.characterModel3D, character.transform);
            modelInstance.transform.localPosition = Vector3.zero;


            if (spawned % 2 == 0)
            {
                character.isEnemy = false;
            }
            else
            {
                character.isEnemy = true;
            }

            Renderer renderer = modelInstance.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                if (character.isEnemy)
                    renderer.material.color = Color.red;
                else
                    renderer.material.color = Color.blue;
            }

            tile.SetContainedEntity(character);

            characters.Add(character);
            spawned++;
        }


        void PrintStats()
        {
            Debug.Log("==== Initiatives ====");
            orderedCharacters = characters.OrderByDescending(c => c.speedStat).ToList();

            foreach (Character c in orderedCharacters)
            {
                Debug.Log($"{c.characterName} -> {c.speedStat}\nID -> {c.id} \nHP -> {c.HP}\nattack -> {c.attackStat}\ndefense -> {c.defenseStat}\nmovement -> {c.movementStat}\nprice -> {c.priceStat}");
            }

        }

        public void HandleEntitySelection(IEntity entity)
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

        }

        private void HandleCharacterDied(Character c)
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
        }


    }
}
