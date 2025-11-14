using System;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Entity.Components;
using CursedOnion.Game.Systems.Grid;
using UnityEngine;
using UnityEngine.Splines;

namespace CursedOnion.Game.Commands
{
    public class MoveCommand : IStackableCommand
    {
        private SimpleEntity commandSubject;
        private Vector3 previousPosition;
        private Vector3 targetPosition;

        private bool previousHasMoved;
        public static MoveCommand Create(CommandParameters parameters)
        {
            try
            {
                if(!parameters.Subject) throw new ArgumentException($"[MoveCommand] No se puede ejecutar: No tiene un CommandSubject");
                if(parameters.Position == null) throw new ArgumentException($"[MoveCommand] No se puede ejecutar: No tiene posición");
            
                return new MoveCommand(parameters.Subject, parameters.Position.Value);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                return null;
            }
        }
        public static void Prepare(CommandParameters parameters)
        { 
            _ = parameters.Subject?.EntityController.GetEntityComponent<MoveEntityComponent>().VisualizeMovement();
        }
        private MoveCommand(SimpleEntity commandSubject, Vector3 newPosition)
        {
            this.commandSubject = commandSubject;
            this.targetPosition = newPosition;
        }
        public bool CanExecute()
        {
            return commandSubject.EntityController.GetEntityComponent<MoveEntityComponent>().ValidateMove(targetPosition).Result;
        }
        public bool Execute()
        {
            if (!CanExecute()) return false;
            
            previousPosition = commandSubject.transform.position;
            commandSubject.EntityController.GetEntityComponent<MoveEntityComponent>().DoMove(targetPosition, false);
            return true;
        }

        public void Undo()
        {
            commandSubject.EntityController.GetEntityComponent<MoveEntityComponent>().DoMove(targetPosition, true);
        }
    }
}
