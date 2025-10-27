using System;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Systems.Grid;
using UnityEngine;
using UnityEngine.Splines;

namespace CursedOnion.Game.Commands
{
    public class MoveCommand : EntityCommand
    {
        private Vector3 previousPosition;
        private Vector3 targetPosition;

        private bool previousHasMoved;

        public static MoveCommand Create(CommandableEntity commandSubject, Vector3 newPosition)
        {
            if(!commandSubject) throw new ArgumentException($"Command subject cannot be null");
            return new MoveCommand(commandSubject, newPosition);
        }
        public static MoveCommand ModifyCommand(MoveCommand moveCommand, Vector3 newPosition)
        {
            moveCommand.targetPosition = newPosition;
            return moveCommand;
        }
        private MoveCommand(CommandableEntity commandSubject, Vector3 newPosition) : base(commandSubject)
        {
            this.targetPosition = newPosition;
        }

        public override bool Execute()
        {
            bool success = CommandSubject.ValidateMove(targetPosition);

            if (success)
            {
                previousPosition = CommandSubject.transform.position;
                CommandSubject.Move(targetPosition);
            }
            return success;
        }

        public override void Undo()
        {
            Debug.Log("El comando de movimiento se ha DESHECHO");
            CommandSubject.UndoMove(previousPosition);
        }

        public override void Redo()
        {
            Debug.Log("El comando de movimiento se ha VUELTO A HACER");
            Execute();
        }
    }
}
