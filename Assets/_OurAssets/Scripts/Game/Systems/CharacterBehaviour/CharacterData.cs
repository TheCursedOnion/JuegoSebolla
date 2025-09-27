using UnityEngine;

namespace CursedOnion
{
    [CreateAssetMenu(fileName = "New CharacterData", menuName = "CharacterData")]

    public class CharacterData : ScriptableObject
    {
        [SerializeField] private string characterName;
        [SerializeField] private int minSpeed;
        [SerializeField] private int maxSpeed;

        public string CharacterName { get { return characterName; } }

        public int GetRandomSpeed()
        {
            return Random.Range(minSpeed, maxSpeed + 1); 
        }
    }
}
