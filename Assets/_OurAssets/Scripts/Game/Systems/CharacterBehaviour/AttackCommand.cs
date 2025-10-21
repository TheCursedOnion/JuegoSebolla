using CursedOnion.Game.Entity;
using CursedOnion.Game.Systems.Grid;
using UnityEngine;

namespace CursedOnion.Game.Commands
{
    public class AttackCommand : EntityCommand
    {
        IEntity target;
        int previousHP;
        private Grid3d grid;
        private Tile3d previousTile;
        private bool wasDeadBefore;

        private bool attackerPreviousHasAttacked;

        public AttackCommand(ICommandEntity entity, IEntity target, Grid3d levelgrid) : base(entity) 
        {
            this.target = target;
            this.grid = levelgrid;
        }

        public override void Execute()
        {
            if (target != null)
            {
                previousHP = target.Stats.CurrentHealthStat;
                wasDeadBefore = target.Flags.HasDied;

                previousTile = grid.GetTileAtWorldPosition(target.Transform.position);
            }
            
            attackerPreviousHasAttacked = Entity.Flags.HasAttacked;
            
            Entity.Attack(target);

            /*var targetChar = target as Character;
            if (targetChar != null && targetChar.hasDied && previousTile != null)
            {
                previousTile.SetContainedEntity(null);
            }*/
        }

        public override void Undo()
        {
            Debug.Log("El comando de ataque se ha DESHECHO");

            if (Entity != null)
            {
                Entity.Flags.HasAttacked = attackerPreviousHasAttacked;
            }

            if (target != null)
            {
                target.Stats.CurrentHealthStat = previousHP;
                target.Flags.HasDied = wasDeadBefore;

                /*if (!wasDeadBefore && previousTile != null)
                {
                    previousTile.SetContainedEntity(targetObj);
                    targetObj.gameObject.SetActive(true);
                    Debug.Log(targetObj.characterName + targetObj.id + " has been healed to HP: " + targetObj.HP);
                }
                targetObj.UpdateCharacterUI();*/
            }
        }

        public override void Redo()
        {
            Debug.Log("El comando de ataque se ha VOLVIDO A HACER");
            Entity.Attack(target);
        }
    }


}
