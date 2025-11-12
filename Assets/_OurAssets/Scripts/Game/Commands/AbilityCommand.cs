using System;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Systems.Grid;
using UnityEngine;

namespace CursedOnion.Game.Commands
{
    public class AbilityCommand : EntityCommand, IClearStackCommand
    {
        private SimpleEntity target;
        public static void Prepare(SimpleEntity subject)
        {
            subject?.EntityController.AbilityEntityComponent.VisualizeAbility();
        }
        public static AbilityCommand Create(SimpleEntity commandSubject, SimpleEntity target)
        {
            if(!commandSubject) throw new ArgumentException($"Command subject cannot be null");
            return new AbilityCommand(commandSubject, target);
        }
        private AbilityCommand(SimpleEntity commandSubject, SimpleEntity target) : base(commandSubject) 
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
            if (!CommandSubject.EntityController.AbilityEntityComponent.ValidateAbility(target))
            {
                Debug.LogWarning($"[AbilityCommand] No se puede ejecutar: {CommandSubject.name} no puede usar la habilidad");
                return false;
            }
            return true;
        }
        public bool Execute()
        {
            if(!CanExecute()) return false;

            CommandSubject.EntityController.AbilityEntityComponent.DoAbility(target, false);
            return true;
        }
    }


}
