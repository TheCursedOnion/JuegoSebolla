using System;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Systems.Grid;
using UnityEngine;

namespace CursedOnion.Game.Commands
{
    public class AbilityCommand : EntityCommand, IClearStackCommand
    {
        private SimpleEntity target;
        
        public static AbilityCommand Create(CommandableEntity commandSubject, SimpleEntity target)
        {
            if(!commandSubject) throw new ArgumentException($"Command subject cannot be null");
            return new AbilityCommand(commandSubject, target);
        }
        private AbilityCommand(CommandableEntity commandSubject, SimpleEntity target) : base(commandSubject) 
        {
            this.target = target;
        }
        
        public bool CanExecute()
        {
            if (!CommandSubject)
            {
                Debug.LogWarning($"[AbilityCommand] No se puede ejecutar: No tiene un CommandSubject");
                return false;
            }
            if (!CommandSubject.ValidateAbility(target))
            {
                Debug.LogWarning($"[AbilityCommand] No se puede ejecutar: {CommandSubject.name} no puede usar la habilidad");
                return false;
            }
            return true;
        }
        public bool Execute()
        {
            if(!CanExecute()) return false;

            CommandSubject.ActivateAbility(target);
            return true;
        }
    }


}
