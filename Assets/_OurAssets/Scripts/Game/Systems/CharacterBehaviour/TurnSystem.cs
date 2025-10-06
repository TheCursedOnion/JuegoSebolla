using CursedOnion.Extensions;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.ScriptableObjects;
using Reflex.Attributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static UnityEngine.GraphicsBuffer;

namespace CursedOnion
{
    public class TurnSystem : MonoBehaviour
    {
        [Inject] private CommandManager commandManager;
        [Inject] private LevelAsset levelAsset;

        [SerializeField] private CharacterData[] characterTypes;
        [SerializeField] private int numberOfCharacters = 15;
        //tests
        [SerializeField] private GameObject characterModel3DTest;
        [SerializeField] private Vector3 levelOffset;

        private List<Character> characters = new List<Character>();
        private List<Character> orderedCharacters = new List<Character>();

        private int currentCharacterIndex = 0;
        private int turnCount = 1;
        private bool waitingForInput = true;

        void Start()
        {
            GenerateCharacters();
            PrintStats();
            StartTurn();
        }

        private void StartTurn()
        {
            if (characters.Count > 0)
            {
                Debug.Log("Turno " + turnCount + " comienza.");
                orderedCharacters[currentCharacterIndex].DoTurn();
            }
        }

        void Update()
        {
            if (waitingForInput && Input.GetKeyDown(KeyCode.Space))
            {
                NextTurn();
            }
            if (waitingForInput && Input.GetKeyDown(KeyCode.A))
            {
                var attackCmd = CharacterCommand.Create<AttackCommand>(orderedCharacters[currentCharacterIndex], orderedCharacters[currentCharacterIndex - 1]);
                commandManager.ExecuteCommand(attackCmd);
            }
            if (waitingForInput && Input.GetKeyDown(KeyCode.M))
            {
                var moveCmd = CharacterCommand.Create<MoveCommand>(orderedCharacters[currentCharacterIndex], new Vector3(1, 1, 1));
                commandManager.ExecuteCommand(moveCmd);
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

        void NextTurn()
        {
            currentCharacterIndex++;

            if (currentCharacterIndex >= orderedCharacters.Count)
            {
                currentCharacterIndex = 0;
                turnCount++;
                Debug.Log("Turno " + turnCount + " comienza.");
            }
            orderedCharacters[currentCharacterIndex].DoTurn();
        }

        void GenerateCharacters()
        {
            // This is all Test code to generate characters in the level
            var levelGrid = levelAsset.Grid;
            var gridSize = levelGrid.Size;      // Vector3Int (x, y, z)
            var gridOrigin = levelGrid.Origin;  // Vector3

            int spawned = 0;
            for (int x = 0; x < gridSize.x && spawned < numberOfCharacters; x++)
            {
                for (int y = 0; y < gridSize.y && spawned < numberOfCharacters; y++)
                {
                    for (int z = 0; z < gridSize.z && spawned < numberOfCharacters; z++)
                    {
                        Vector3 gridPosition = new Vector3(x, y, z);
                        Tile3d tile = levelGrid.GetTileAtGridPosition(gridPosition);

                        if (tile != null && levelGrid.TryGridToWorldPosition(gridPosition, out Vector3 worldPosition))
                        {
                            GameObject charObj = new GameObject("Character_" + spawned);

                            charObj.transform.parent = this.transform;
                            charObj.transform.position = worldPosition.Center();

                            Character character = charObj.AddComponent<Character>();
                            character.characterModel3D = characterModel3DTest;
                            
                            GameObject modelInstance = Instantiate(character.characterModel3D, character.transform);
                            modelInstance.transform.localPosition = Vector3.zero;

                            CharacterData randomType = characterTypes[Random.Range(0, characterTypes.Length)];
                            character.characterModel3D = characterModel3DTest; 
                            character.data = randomType;
                            character.id = spawned;
                            character.SetCharacterData();

                            //tile.containedEntity = character; 

                            characters.Add(character);
                            spawned++;
                        }

                    }
                }
            }
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
                }
            }
