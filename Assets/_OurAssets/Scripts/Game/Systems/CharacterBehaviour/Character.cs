using UnityEngine;

namespace CursedOnion
{
    public class Character : MonoBehaviour
    {
        // Character type 
        public CharacterData data;

        // Character Stats
        public string characterName { get; set; }
        public int HP { get; set; }
        public int attackStat { get; set; }
        public int defenseStat { get; set; }
        public int speedStat { get; set; }
        public int movementStat { get; set; }
        public int priceStat { get; set; }
        public int id { get; set; }

        public void SetCharacterData()
        {
            characterName = data.SetCharacterName();
            HP = data.SetRandomHP();
            attackStat = data.SetRandomAttack();
            defenseStat = data.SetRandomDefense();
            speedStat = data.SetRandomSpeed();
            movementStat = data.SetMovement();
            priceStat = data.SetPrice();
        }

        public void DoTurn()
        {
            Debug.Log(characterName + " id: " + id + " está haciendo su turno...");
        }
    }
}
