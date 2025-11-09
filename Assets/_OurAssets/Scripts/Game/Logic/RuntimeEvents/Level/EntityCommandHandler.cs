using System;
using System.Diagnostics;
using System.Reflection;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Events;
using CursedOnion.Game.Systems.Grid;
using Reflex.Attributes;
using Reflex.Core;
using Reflex.Injectors;
using UnityEngine;

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

            if (commandSubject is Unit unitSubject)
            {
                Grid3d grid = unitSubject.GetGrid();
                Vector3 unitPos = unitSubject.transform.position;
                int moveRange = unitSubject.GetStats().MovementStat;
                grid.HighlightMovementRange(unitPos, moveRange, Color.blue);
            }
        }

        public void ExecuteCommand(CommandParameters parameters)
        {
            LaunchCommand(parameters);
            
            if(preparedParameters != null && preparedParameters.ExecuteOnce) ResetCommand();
        }
        void LaunchCommand(CommandParameters parameters)
        {
            if(preparedCommand == null) return;
            
            var createMethod = typeof(CommandFactory).GetMethod("Create", BindingFlags.Public | BindingFlags.Static);
            var genericMethod = createMethod.MakeGenericMethod(preparedCommand);
            
            CommandParameters.CombineParameters(parameters, preparedParameters);
            
            parameters.Subject = commandSubject;
            var command = genericMethod.Invoke(null, new object[] { parameters });
            
            if(command != null) commandManager.ExecuteCommand((ICommand)command);
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
