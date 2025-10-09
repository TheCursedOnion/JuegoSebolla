using CursedOnion.Extensions;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.Helpers;
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
            Renderer meshRenderer = GetComponent<Renderer>();
            levelOffset = levelAsset.Grid.Origin - meshRenderer.bounds.min;

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
            if (waitingForInput && Input.GetKey(KeyCode.A) && Input.GetMouseButtonDown(0))
            {
                TryToAttackCharacter();
            }
            if (waitingForInput && Input.GetKey(KeyCode.M) && Input.GetMouseButtonDown(0))
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

        void NextTurn()
        {
            do
            {
                currentCharacterIndex++;
                if (currentCharacterIndex >= orderedCharacters.Count)
                {
                    currentCharacterIndex = 0;
                    turnCount++;
                    Debug.Log("Turno " + turnCount + " comienza.");
                }
            }
            while (orderedCharacters[currentCharacterIndex].hasDied);

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
                for (int z = 0; z < gridSize.z && spawned < numberOfCharacters; z++)
                {
                    Vector3 gridPosition = new Vector3(x, 0, z);
                    Tile3d tile = levelGrid.GetTileAtGridPosition(gridPosition);

                    if (tile != null && levelGrid.TryGridToWorldPosition(gridPosition, out Vector3 worldPosition))
                    {
                        GameObject charObj = new GameObject("Character_" + spawned);
                        
                        charObj.transform.position = worldPosition.Center() + new Vector3(0f, 1.0f, 0f);

                        Character character = charObj.AddComponent<Character>();
                        character.characterModel3D = characterModel3DTest;

                        GameObject modelInstance = Instantiate(character.characterModel3D, character.transform);
                        modelInstance.transform.localPosition = Vector3.zero;

                        CharacterData randomType = characterTypes[Random.Range(0, characterTypes.Length)];
                        character.characterModel3D = characterModel3DTest;
                        character.data = randomType;
                        character.id = spawned;
                        character.SetCharacterData();

                        tile.SetContainedEntity(character);

                        characters.Add(character);
                        spawned++;
                    }

                }
            }
        }


        private void TryToMoveCharacter()
        {
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
                        Debug.Log("Not valid tile: " + gridPosition);
                    }
                }
            }
        }

        private void TryToAttackCharacter()
        {
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
                        var attackCmd = CharacterCommand.Create<AttackCommand>(orderedCharacters[currentCharacterIndex], aboveTile.GetContainedEntity(), levelGrid);
                        commandManager.ExecuteCommand(attackCmd);
                    }
                    else
                    {
                        Debug.Log("Not valid tile: " + gridPosition);
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
