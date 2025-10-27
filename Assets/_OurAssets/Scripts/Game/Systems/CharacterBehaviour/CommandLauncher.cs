using System;
using System.Reflection;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Events;
using CursedOnion.Game.Handlers;
using CursedOnion.Game.Systems.Grid;
using UnityEngine;

namespace CursedOnion.Game.Commands
{
    public enum CommandType
    {
        None,
        Move,
        Attack,
    }
    public class CommandLauncher
    {
        private CommandableEntity commandSubject;
        private Type preparedCommand;

        private readonly CommandManager commandManager;

        public CommandLauncher(CommandManager commandManager)
        {
            this.commandManager = commandManager;
        }
        
        public bool SetCommandSubject(CommandableEntity commandableEntity)
        {
            if(commandableEntity ==null || HasPreparedCommand()) return false;
            
            CancelCommand();
            commandSubject = commandableEntity;
            return true;
        }
        public void PrepareCommand<T>() where T : EntityCommand
        {
            if(commandSubject == null) return;
            Debug.Log($"Preparing command {typeof(T).Name}");
            preparedCommand = typeof(T);
        }
        public bool HasPreparedCommand()
        {
            return preparedCommand != null;
        }
        
        public void CancelCommand()
        {
            preparedCommand = null;
            commandSubject = null;
        }
        public bool LaunchCommand(EntityCommandParameters parameters)
        {
            if(preparedCommand == null) return false;
            Debug.Log($"Launching command {preparedCommand.Name}");
            
            var createMethod = typeof(EntityCommand).GetMethod("Create", BindingFlags.Public | BindingFlags.Static);
            var genericMethod = createMethod.MakeGenericMethod(preparedCommand);

            var command = genericMethod.Invoke(null, new object[] { commandSubject, parameters });

            bool wasCommandSuccessful = commandManager.ExecuteCommand((EntityCommand)command);
            CancelCommand();
            return wasCommandSuccessful;
        }
    }
}
