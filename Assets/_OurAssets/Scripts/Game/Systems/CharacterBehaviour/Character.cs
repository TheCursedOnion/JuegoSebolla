using UnityEngine;

namespace CursedOnion
{
    public class Character : MonoBehaviour
    {
        // Character type 
        public CharacterData data;

        // Character Stats
        public int HP { get; set; }
        public int attackStat { get; set; }
        public int defenseStat { get; set; }
        public int speedStat { get; set; }
        public int movementStat { get; set; }
        public int priceStat { get; set; }

        public void SetCharacterData()
        {
            HP = data.SetRandomHP();
            attackStat = data.SetRandomAttack();
            defenseStat = data.SetRandomDefense();
            speedStat = data.SetRandomSpeed();
            movementStat = data.SetMovement();
            priceStat = data.SetPrice();
        }
    }
}
