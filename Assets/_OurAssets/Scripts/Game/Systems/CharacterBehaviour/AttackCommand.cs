using System;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Systems.Grid;
using UnityEngine;

namespace CursedOnion.Game.Commands
{
    public class AttackCommand : EntityCommand, IClearStackCommand
    {
        private SimpleEntity target;
        
        private bool targetHasDied = false;
        private int previousHp;
        

        private bool attackerPreviousHasAttacked;
        
        public static AttackCommand Create(CommandableEntity commandSubject, SimpleEntity target)
        {
            if(!commandSubject) throw new ArgumentException($"Command subject cannot be null");
            return new AttackCommand(commandSubject, target);
        }
        private AttackCommand(CommandableEntity commandSubject, SimpleEntity target) : base(commandSubject) 
        {
            this.target = target;
        }

        public bool Execute()
        {
            bool success = CommandSubject.ValidateAttack(target);
            if (success)
            {
                previousHp = target.GetStats().CurrentHealthStat;
                CommandSubject.Attack(target);
                targetHasDied = target.GetFlags().HasDied;
            }
            return success;
        }
    }


}
