using System;
using UnityEngine;

namespace CursedOnion.Game.Commands
{
    public class ActionCommand : ICommand
    {
        private Action action;
        public static ActionCommand Create(Action action)
        {
            return new ActionCommand(action);
        }
        private ActionCommand(Action action)
        {
            this.action = action;
        }

        public void OnPrepare()
        {
            
        }

        public bool CanExecute()
        {
            if (action == null)
            {
                Debug.LogWarning("[ActionCommand] No se puede ejecutar: action es nula");
                return false;
            }
            return true;
        }
        public bool Execute()
        {
            if(CanExecute()) action();
            return true;
        }
    }
}