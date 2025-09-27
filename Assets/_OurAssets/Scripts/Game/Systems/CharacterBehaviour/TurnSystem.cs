using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace CursedOnion
{
    public class TurnSystem : MonoBehaviour
    {
        [SerializeField] private CharacterData[] characterTypes;
        [SerializeField] private int numberOfCharacters = 7;

        private List<Character> characters = new List<Character>();

        void Start()
        {
            GenerateCharacters();
            PrintInitiatives();
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
                character.initiative = character.data.GetRandomSpeed();

                characters.Add(character);
            }
        }

        void PrintInitiatives()
        {
            Debug.Log("==== Initiatives ====");
            var orderedCharacters = characters.OrderByDescending(c => c.initiative).ToList();

            foreach (Character c in orderedCharacters)
            {
                Debug.Log($"{c.data.CharacterName} -> {c.initiative}");
            }
        }
    }
}
