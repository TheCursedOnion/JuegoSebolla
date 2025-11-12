using System;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Systems.Grid;
using UnityEngine;
using UnityEngine.Splines;

namespace CursedOnion.Game.Commands
{
    public class MoveCommand : EntityCommand, IStackableCommand
    {
        private Vector3 previousPosition;
        private Vector3 targetPosition;

        private bool previousHasMoved;

        public static MoveCommand Create(CommandableEntity commandSubject, Vector3 newPosition)
        {
            return new MoveCommand(commandSubject, newPosition);
        }
        public static void Prepare(CommandableEntity subject)
        {
            subject?.EntityController.MoveEntityComponent.VisualizeMovement();
        }
        private MoveCommand(CommandableEntity commandSubject, Vector3 newPosition) : base(commandSubject)
        {
            this.targetPosition = newPosition;
        }
        
        
        public bool CanExecute()
        {
            if (!CommandSubject)
            {
                Debug.LogWarning($"[MoveCommand] No se puede ejecutar: No tiene un CommandSubject");
                return false;
            }
            if (!CommandSubject.EntityController.MoveEntityComponent.ValidateMove(targetPosition).Result)
            {
                Debug.LogWarning($"[MoveCommand] No se puede ejecutar: {CommandSubject.name} no puede moverse a {targetPosition}");
                return false;
            }
            return true;
        }
        public bool Execute()
        {
            if (!CanExecute()) return false;
            
            previousPosition = CommandSubject.transform.position;
            CommandSubject.EntityController.MoveEntityComponent.DoMove(targetPosition, false);
            return true;
        }

        public void Undo()
        {
            CommandSubject.EntityController.MoveEntityComponent.DoMove(targetPosition, true);
        }
    }
}
