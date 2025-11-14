using System;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Entity.Components;
using CursedOnion.Game.Systems.Grid;
using UnityEngine;

namespace CursedOnion.Game.Commands
{
    public class AbilityCommand : IClearStackCommand
    {
        private SimpleEntity commandSubject;
        private SimpleEntity target;
        public static void Prepare(CommandParameters parameters)
        {
            parameters.Subject?.EntityController.GetEntityComponent<SpecialAbilityComponent>().VisualizeAbility();
        }
        public static AbilityCommand Create(CommandParameters parameters)
        {
            try
            {
                if(!parameters.Subject) throw new ArgumentException($"[AbilityCommand] Command subject cannot be null");
                if(!parameters.Target) throw new ArgumentException($"[AbilityCommand] Target cannot be null");
            
                return new AbilityCommand(parameters.Subject, parameters.Target);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                return null;
            }
        }
        private AbilityCommand(SimpleEntity commandSubject, SimpleEntity target)
        {
            this.commandSubject = commandSubject;
            this.target = target;
        }
        public bool CanExecute()
        {
            return commandSubject.EntityController.GetEntityComponent<SpecialAbilityComponent>().ValidateAbility(target);
        }
        public bool Execute()
        {
            if(!CanExecute()) return false;

            commandSubject.EntityController.GetEntityComponent<SpecialAbilityComponent>().DoAbility(target, false);
            return true;
        }
    }


}
