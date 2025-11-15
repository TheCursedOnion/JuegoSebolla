using System;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Entity.Components;
using CursedOnion.Game.Systems.Grid;
using UnityEngine;

namespace CursedOnion.Game.Commands
{
    public class AttackCommand : IClearStackCommand
    {
        private SimpleEntity commandSubject;
        private SimpleEntity target;

        
        public static AttackCommand Create(CommandParameters parameters)
        {
            try
            {
                if(!parameters.Subject) throw new ArgumentException($"[AttackCommand] No se puede ejecutar: No tiene un CommandSubject");

                return new AttackCommand(parameters.Subject, parameters.Target);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                return null;
            }
        }
        private AttackCommand(SimpleEntity commandSubject, SimpleEntity target)
        {
            this.commandSubject = commandSubject;
            this.target = target;
        }
        public static void Prepare(CommandParameters parameters)
        {
            parameters.Subject?.EntityController.GetEntityComponent<AttackEntityComponent>().VisualizeAttack();
        }
        
        public bool CanExecute()
        {
            return commandSubject.EntityController.GetEntityComponent<AttackEntityComponent>().ValidateAttack(target);
        }
        public bool Execute()
        {
            if(!CanExecute()) return false;

            commandSubject.EntityController.GetEntityComponent<AttackEntityComponent>().DoAttack(target, false);
            return true;
        }
    }


}
