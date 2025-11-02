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
            if (!CommandSubject.ValidateMove(targetPosition))
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
            CommandSubject.Move(targetPosition);
            return true;
        }

        public void Undo()
        {
            Debug.Log("El comando de movimiento se ha DESHECHO");
            CommandSubject.UndoMove(previousPosition);
        }
    }
}
