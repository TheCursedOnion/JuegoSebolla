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
            if(!commandSubject) throw new ArgumentException($"Command subject cannot be null");
            return new MoveCommand(commandSubject, newPosition);
        }
        private MoveCommand(CommandableEntity commandSubject, Vector3 newPosition) : base(commandSubject)
        {
            this.targetPosition = newPosition;
        }

        public bool Execute()
        {
            bool success = CommandSubject.ValidateMove(targetPosition);

            if (success)
            {
                previousPosition = CommandSubject.transform.position;
                CommandSubject.Move(targetPosition);
            }
            return success;
        }

        public void Undo()
        {
            Debug.Log("El comando de movimiento se ha DESHECHO");
            CommandSubject.UndoMove(previousPosition);
        }
    }
}
