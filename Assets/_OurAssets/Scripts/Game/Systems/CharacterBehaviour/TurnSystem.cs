using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace CursedOnion
{
    public class TurnSystem : MonoBehaviour
    {
        [SerializeField] private CharacterData[] characterTypes;
        [SerializeField] private int numberOfCharacters = 15;

        private List<Character> characters = new List<Character>();

        void Start()
        {
            GenerateCharacters();
            PrintStats();
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
                character.SetCharacterData();

                characters.Add(character);
            }
        }

        void PrintStats()
        {
            Debug.Log("==== Initiatives ====");
            var orderedCharacters = characters.OrderByDescending(c => c.speedStat).ToList();

            foreach (Character c in orderedCharacters)
            {
                Debug.Log($"{c.data.CharacterName} -> {c.speedStat}\nHP -> {c.HP}\nattack -> {c.attackStat}\ndefense -> {c.defenseStat}\nmovement -> {c.movementStat}\nprice -> {c.priceStat}");
            }
        }
    }
}
