using System;
using UnityEngine;

namespace CursedOnion.Game.Commands
{
    public class ActionCommand : ICommand
    {
        private Action action;
        public static ActionCommand Create(CommandParameters parameters)
        {
            try
            {
                if(parameters.ExecuteAction == null) throw new ArgumentException($"[ActionCommand] Action cannot be null");
                return new ActionCommand(parameters.ExecuteAction);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                throw;
            }
            
        }
        private ActionCommand(Action action)
        {
            this.action = action;
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