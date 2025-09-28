using UnityEngine;

namespace CursedOnion
{
    [CreateAssetMenu(fileName = "New CharacterData", menuName = "CharacterData")]

    public class CharacterData : ScriptableObject
    {
        [SerializeField] private string characterName;

        [SerializeField] private int minHP;
        [SerializeField] private int maxHP;
        [SerializeField] private int minAttack;
        [SerializeField] private int maxAttack;
        [SerializeField] private int minDefense;
        [SerializeField] private int maxDefense;
        [SerializeField] private int minSpeed;
        [SerializeField] private int maxSpeed;
        [SerializeField] private int movement;
        [SerializeField] private int price;

        public string CharacterName { get { return characterName; } }

        public int SetRandomHP()
        {
            return Random.Range(minHP, maxHP + 1);
        }
        public int SetRandomSpeed()
        {
            return Random.Range(minSpeed, maxSpeed + 1); 
        }
        public int SetRandomAttack()
        {
            return Random.Range(minAttack, maxAttack + 1);
        }
        public int SetRandomDefense()
        {
            return Random.Range(minDefense, maxDefense + 1);
        }
        public int SetMovement()
        {
            return movement;
        }
        public int SetPrice()
        {
            return price;
        }
    }
}
