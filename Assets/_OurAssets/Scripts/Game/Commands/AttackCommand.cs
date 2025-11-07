using System;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Systems.Grid;
using UnityEngine;

namespace CursedOnion.Game.Commands
{
    public class AttackCommand : EntityCommand, IClearStackCommand
    {
        private SimpleEntity target;
        
        public static AttackCommand Create(CommandableEntity commandSubject, SimpleEntity target)
        {
            if(!commandSubject) throw new ArgumentException($"Command subject cannot be null");
            return new AttackCommand(commandSubject, target);
        }
        private AttackCommand(CommandableEntity commandSubject, SimpleEntity target) : base(commandSubject) 
        {
            this.target = target;
        }
        
        public bool CanExecute()
        {
            if (!CommandSubject)
            {
                Debug.LogWarning($"[AttackCommand] No se puede ejecutar: No tiene un CommandSubject");
                return false;
            }
            if (!CommandSubject.ValidateAttack(target))
            {
                Debug.LogWarning($"[MoveCommand] No se puede ejecutar: {CommandSubject.name} no puede moverse atacar");
                return false;
            }
            return true;
        }
        public bool Execute()
        {
            if(!CanExecute()) return false;

            CommandSubject.Attack(target);
            return true;
        }
    }


}
