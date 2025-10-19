using CursedOnion.Game.Systems.Grid;
using UnityEngine;

namespace CursedOnion
{
    public class AttackCommand : CharacterCommand
    {
        IEntity target;
        int previousHP;
        private Grid3d grid;
        private Tile3d previousTile;
        private bool wasDeadBefore;

        private bool attackerPreviousHasAttacked;

        public AttackCommand(IEntity character, IEntity target, Grid3d levelgrid) : base(character) 
        {
            this.target = target;
            this.grid = levelgrid;
        }

        public override void Execute()
        {
            var targetObj = target as Character;
            if (targetObj != null)
            {
                previousHP = targetObj.HP;
                wasDeadBefore = targetObj.hasDied;

                previousTile = grid.GetTileAtWorldPosition(targetObj.transform.position);
            }

            var characterObj = character as Character;
            if (characterObj != null)
            {
                attackerPreviousHasAttacked = characterObj.hasAttacked;
            }
            character.Attack(target);

            var targetChar = target as Character;
            if (targetChar != null && targetChar.hasDied && previousTile != null)
            {
                previousTile.SetContainedEntity(null);
            }
        }

        public override void Undo()
        {
            var characterObj = character as Character;
            var targetObj = target as Character;

            Debug.Log("El comando de ataque se ha DESHECHO");

            if (characterObj != null)
            {
                characterObj.hasAttacked = attackerPreviousHasAttacked;
                characterObj.UpdateCharacterUI();
            }

            if (targetObj != null)
            {
                targetObj.HP = previousHP;
                targetObj.hasDied = wasDeadBefore;

                if (!wasDeadBefore && previousTile != null)
                {
                    previousTile.SetContainedEntity(targetObj);
                    targetObj.gameObject.SetActive(true);
                    Debug.Log(targetObj.characterName + targetObj.id + " has been healed to HP: " + targetObj.HP);
                }
                targetObj.UpdateCharacterUI();
            }
        }

        public override void Redo()
        {
            Debug.Log("El comando de ataque se ha VOLVIDO A HACER");
            character.Attack(target);
        }
    }


}
