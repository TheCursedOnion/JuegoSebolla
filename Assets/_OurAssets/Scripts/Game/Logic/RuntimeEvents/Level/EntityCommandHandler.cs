using System;
using System.Reflection;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Events;
using Reflex.Attributes;
using Reflex.Core;
using Reflex.Injectors;

namespace CursedOnion.Game.Commands
{
    public class EntityCommandHandler : IDisposable
    {
        [Inject] private readonly LevelEvents levelEvents;
        [Inject] private readonly CommandManager commandManager;
        
        private SimpleEntity selectedEntity;
        private CommandableEntity commandSubject;
        
        private Type preparedCommand;
        
        private CommandParameters preparedParameters;
        
        
        public EntityCommandHandler(Container sceneContainer)
        {
            AttributeInjector.Inject(this, sceneContainer);
            
            levelEvents.OnEntitySelected += SelectEntity;
            levelEvents.OnCommandPrepareCalled += PrepareCommand;
            levelEvents.OnPreparedCommandCancelled += ResetCommand;
        }
        public void Dispose()
        {
            levelEvents.OnEntitySelected -= SelectEntity;
            levelEvents.OnCommandPrepareCalled -= PrepareCommand;
            levelEvents.OnPreparedCommandCancelled -= ResetCommand;
        }
        private void SelectEntity(SimpleEntity entity)
        {
            selectedEntity = entity;
            SetCommandSubject();
        }
        private void SetCommandSubject()
        {
            var commandableEntity = selectedEntity as CommandableEntity;
            
            if(commandableEntity == null) ResetCommand();
            else if(commandSubject != commandableEntity) commandManager.ClearStack();
            
            commandSubject = commandableEntity;
        }

        
        public bool HasPreparedCommand()
        {
            return preparedCommand != null;
        }
        
        void PrepareCommand(Type commandType, CommandParameters parameters)
        {
            preparedCommand = commandType;
            preparedParameters = parameters;
        }
        
        public void ExecuteCommand(CommandParameters parameters)
        {
            LaunchCommand(parameters);
            ResetCommand();
        }
        public void TriggerCommand(CommandParameters parameters)
        {
            LaunchCommand(parameters);
        }
        void LaunchCommand(CommandParameters parameters)
        {
            if(preparedCommand == null) return;
            
            var createMethod = typeof(CommandFactory).GetMethod("Create", BindingFlags.Public | BindingFlags.Static);
            var genericMethod = createMethod.MakeGenericMethod(preparedCommand);
            
            parameters.Combine(preparedParameters);
            
            parameters.Subject = commandSubject;
            var command = genericMethod.Invoke(null, new object[] { parameters });

            commandManager.ExecuteCommand((ICommand)command);
        }
        private void ResetCommand()
        {
            preparedCommand = null;
            preparedParameters = null;
            
            levelEvents.SelectEntity(null);
        }
        
        public void ClearCommandStack()
        {
            commandManager.ClearStack();
        }

        
    }
}
