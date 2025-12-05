using System.Collections.Generic;
using UnityEngine;

namespace CursedOnion.Game.Commands
{
    public class CommandManager
    {
        private Stack<ICommand> undoStack = new Stack<ICommand>();
        public bool ExecuteCommand(ICommand command)
        {
            bool success = command.Execute();
            
            if (success) 
                switch (command)
                {
                    case IStackableCommand:
                        //Debug.Log("Pusheo al Stack");
                        undoStack.Push(command);
                        break;
                    case IClearStackCommand:
                        ClearStack();
                        break;
                }
            
            return success;
        }
        public bool HasCommandsStacked() => undoStack.Count > 0;
        public void Undo()
        {
            if (undoStack.Count > 0)
            {
                var command = (IStackableCommand) undoStack.Pop();
                command.Undo();
            }
        }

        public void ClearStack()
        {
            //Debug.Log("Borro el Stack");
            undoStack.Clear();
        }
    }
}
