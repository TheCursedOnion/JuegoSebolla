using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static UnityEngine.GraphicsBuffer;

namespace CursedOnion
{
    public class TurnSystem : MonoBehaviour
    {
        [SerializeField] private CommandManager commandManager;
        [SerializeField] private CharacterData[] characterTypes;
        [SerializeField] private int numberOfCharacters = 15;

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
                var moveCmd = CharacterCommand.Create<MoveCommand>(orderedCharacters[currentCharacterIndex]);
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
            for (int i = 0; i < numberOfCharacters; i++)
            {
                GameObject charObj = new GameObject("Character_" + i);
                charObj.transform.parent = this.transform;

                Character character = charObj.AddComponent<Character>();
                CharacterData randomType = characterTypes[Random.Range(0, characterTypes.Length)];
                character.data = randomType;
                character.id = i;
                character.SetCharacterData();

                characters.Add(character);
            }
        }

        void PrintStats()
        {
            Debug.Log("==== Initiatives ====");
            orderedCharacters = characters.OrderByDescending(c => c.speedStat).ToList();

            foreach (Character c in orderedCharacters)
            {
                Debug.Log($"{c.characterName} -> {c.speedStat}\nHP -> {c.HP}\nattack -> {c.attackStat}\ndefense -> {c.defenseStat}\nmovement -> {c.movementStat}\nprice -> {c.priceStat}");
            }

        }
    }
}
