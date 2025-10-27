using System.Collections.Generic;
using UnityEngine;

namespace CursedOnion.Game.Commands
{
    public class CommandManager
    {
        private Stack<IStackableCommand> undoStack = new Stack<IStackableCommand>();
        private Stack<IStackableCommand> redoStack = new Stack<IStackableCommand>();

        public bool ExecuteCommand(IStackableCommand command)
        {
            bool success = command.Execute();
            if (success)
            {
                undoStack.Push(command);
                redoStack.Clear();
            }
            return success;
        }

        public void Undo()
        {
            if (undoStack.Count > 0)
            {
                var command = undoStack.Pop();
                command.Undo();
                redoStack.Push(command);
            }
        }

        public void Redo()
        {
            if (redoStack.Count > 0)
            {
                var command = redoStack.Pop();
                command.Redo();
                undoStack.Push(command);
            }
        }

        public void Clear()
        {
            redoStack.Clear();
            undoStack.Clear();
        }
    }
}
