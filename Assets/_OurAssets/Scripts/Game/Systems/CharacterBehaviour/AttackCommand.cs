using System;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Systems.Grid;
using UnityEngine;

namespace CursedOnion.Game.Commands
{
    public class AttackCommand : EntityCommand
    {
        private readonly SimpleEntity target;
        
        private bool targetHasDied = false;
        private int previousHp;
        

        private bool attackerPreviousHasAttacked;
        
        public static AttackCommand Create(CommandableEntity commandSubject, SimpleEntity target)
        {
            if(!commandSubject) throw new ArgumentException($"Command subject cannot be null");
            if(!target) throw new ArgumentException($"Target cannot be null");
            
            return new AttackCommand(commandSubject, target);
        }
        private AttackCommand(CommandableEntity commandSubject, SimpleEntity target) : base(commandSubject) 
        {
            this.target = target;
        }

        public override void Execute()
        {
            previousHp = target.GetStats().CurrentHealthStat;
            CommandSubject.Attack(target);
            targetHasDied = target.HasDied();
        }

        public override void Undo()
        {
            Debug.Log("El comando de ataque se ha DESHECHO");

            if (targetHasDied)
            {
                target.Revive(previousHp);
            }
            CommandSubject.UndoAttack(target);
        }

        public override void Redo()
        {
            Debug.Log("El comando de ataque se ha VOLVIDO A HACER");
            Execute();
        }
    }


}
