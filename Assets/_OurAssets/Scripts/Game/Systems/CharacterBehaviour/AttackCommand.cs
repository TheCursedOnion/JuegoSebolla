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
            character.Attack(target);

            var targetChar = target as Character;
            if (targetChar != null && targetChar.hasDied && previousTile != null)
            {
                previousTile.SetContainedEntity(null);
            }
        }

        public override void Undo()
        {
            Debug.Log("El comando de ataque se ha DESHECHO");
            var targetObj = target as Character;
            if (targetObj != null)
            {
                targetObj.HP = previousHP;
                targetObj.hasDied = wasDeadBefore;

                if (!wasDeadBefore && previousTile != null)
                {
                    previousTile.SetContainedEntity(targetObj);
                    targetObj.gameObject.SetActive(true);
                    Debug.Log(targetObj.characterName + targetObj.id + " has been revived to HP: " + targetObj.HP);
                }
            }
        }

        public override void Redo()
        {
            Debug.Log("El comando de ataque se ha VOLVIDO A HACER");
            character.Attack(target);
        }
    }


}
