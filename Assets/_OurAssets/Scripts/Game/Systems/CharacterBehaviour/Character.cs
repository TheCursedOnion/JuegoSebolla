using UnityEngine;

namespace CursedOnion
{
    public class Character : MonoBehaviour, IEntity
    {
        // Character type 
        public CharacterData data;

        // Character Model (test)
        public GameObject characterModel3D;

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
            Debug.Log(characterName + " id: " + id + " est� haciendo su turno...");
        }

        public void Attack(IEntity target)
        {
            var targetObj = target as Character;
            Debug.Log(characterName + id + " Attacking! " + targetObj.characterName + targetObj.id);
        }

        public void Move(Vector3 newPosition) 
        {
            Debug.Log(characterName + id + " Moving to " + newPosition);
        }
    }
}
