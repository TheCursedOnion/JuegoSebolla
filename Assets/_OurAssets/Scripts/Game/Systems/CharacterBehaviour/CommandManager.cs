using System.Collections.Generic;
using UnityEngine;

namespace CursedOnion
{
    public class CommandManager
    {
        private Stack<IStackableCommand> undoStack = new Stack<IStackableCommand>();
        private Stack<IStackableCommand> redoStack = new Stack<IStackableCommand>();

        public void ExecuteCommand(IStackableCommand command)
        {
            command.Execute();
            undoStack.Push(command);
            redoStack.Clear();
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

        public void ClearTurn()
        {
            redoStack.Clear();
            undoStack.Clear();
        }
    }
}
