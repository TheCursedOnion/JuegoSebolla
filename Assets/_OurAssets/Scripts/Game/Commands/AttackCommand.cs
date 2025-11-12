using System;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Systems.Grid;
using UnityEngine;

namespace CursedOnion.Game.Commands
{
    public class AttackCommand : EntityCommand, IClearStackCommand
    {
        private SimpleEntity target;

        
        public static AttackCommand Create(SimpleEntity commandSubject, SimpleEntity target)
        {
            if(!commandSubject) throw new ArgumentException($"Command subject cannot be null");
            return new AttackCommand(commandSubject, target);
        }
        private AttackCommand(SimpleEntity commandSubject, SimpleEntity target) : base(commandSubject) 
        {
            this.target = target;
        }
        public static void Prepare(SimpleEntity subject)
        {
            subject?.EntityController.AttackEntityComponent.VisualizeAttack();
        }
        
        public bool CanExecute()
        {
            if (!CommandSubject)
            {
                Debug.LogWarning($"[AttackCommand] No se puede ejecutar: No tiene un CommandSubject");
                return false;
            }
            if (!CommandSubject.EntityController.AttackEntityComponent.ValidateAttack(target))
            {
                Debug.LogWarning($"[AttackCommand] No se puede ejecutar: {CommandSubject.name} no puede atacar");
                return false;
            }
            return true;
        }
        public bool Execute()
        {
            if(!CanExecute()) return false;

            CommandSubject.EntityController.AttackEntityComponent.DoAttack(target, false);
            return true;
        }
    }


}
