using System;
using CursedOnion.Game.Commands;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Events;
using CursedOnion.Game.Systems.Grid;

using UnityEngine;

namespace CursedOnion.Game.Handlers
{
    public class EntityCommandHandler
    {
        private readonly CommandManager commandManager;
        private readonly CommandLauncher commandLauncher;
        public EntityCommandHandler()
        {
            commandManager = new();
            commandLauncher = new(commandManager);
        }
        
        public event Action<SimpleEntity> OnEntitySelected;
        public void TrySelectEntity(SimpleEntity entity)
        {
            if(entity is CommandableEntity commandableEntity)
            {
                commandLauncher.SetCommandSubject(commandableEntity);
            }
            else if(entity !=null)
            {
                commandLauncher.CancelCommand();
            }
            
            OnEntitySelected?.Invoke(entity);
        }
        
        public bool HasPreparedCommand()
        {
            return commandLauncher.HasPreparedCommand();
        }
        public void PrepareEntityCommand<T>() where T : EntityCommand
        {
            commandLauncher.PrepareCommand<T>();
        }
        
        public void LaunchPreparedCommandWithParameters(EntityCommandParameters commandParameters)
        {
            commandLauncher.LaunchCommand(commandParameters);
            TrySelectEntity(null);
        }

        public void CancelEntityCommand()
        {
            commandLauncher.CancelCommand();
        }

        public void ClearCommandStack()
        {
            commandManager.Clear();
        }

    }
}
