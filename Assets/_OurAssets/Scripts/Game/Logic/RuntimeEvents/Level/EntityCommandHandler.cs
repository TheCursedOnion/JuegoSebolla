using CursedOnion.Game.Entity;
using CursedOnion.Game.Events;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.Helpers;
using Reflex.Attributes;
using Reflex.Core;
using Reflex.Injectors;
using System;
using System.Diagnostics;
using System.Reflection;
using CursedOnion.Game.Modes.General.UI.Events;
using CursedOnion.Game.Systems.Level;
using UnityEngine;
using UnityEngine.TestTools;
using Debug = UnityEngine.Debug;

namespace CursedOnion.Game.Commands
{
    public class EntityCommandHandler : IDisposable
    {
        [Inject] private readonly LevelEvents levelEvents;
        [Inject] private readonly UIEvents uiEvents;
        [Inject] private readonly CommandManager commandManager;
        
        private SimpleEntity selectedEntity;
        
        private Type preparedCommand;
        
        private CommandParameters preparedParameters;
        
        
        public EntityCommandHandler(Container sceneContainer)
        {
            AttributeInjector.Inject(this, sceneContainer);
            
            levelEvents.OnEntitySelected += SelectEntity;
            levelEvents.OnCommandPrepareCalled += PrepareCommand;
            levelEvents.OnPreparedCommandCancelled += ResetCommand;
            levelEvents.OnTurnEnded += ClearCommandStack;
        }
        public void Dispose()
        {
            levelEvents.OnEntitySelected -= SelectEntity;
            levelEvents.OnCommandPrepareCalled -= PrepareCommand;
            levelEvents.OnPreparedCommandCancelled -= ResetCommand;
            levelEvents.OnTurnEnded -= ClearCommandStack;
        }
        private void SelectEntity(SimpleEntity entity)
        {
            if(entity ==null) ResetCommand();
            else if(selectedEntity != entity) commandManager.ClearStack();
            
            selectedEntity = entity;
        }
        public bool HasPreparedCommand()
        {
            return preparedCommand != null;
        }

        void PrepareCommand(Type commandType, CommandParameters parameters)
        {
            preparedCommand = commandType;
            preparedParameters = parameters;
            
            var previsualizeMethod = typeof(CommandFactory).GetMethod("PreVisualize", BindingFlags.Public | BindingFlags.Static);
            var genericMethod = previsualizeMethod.MakeGenericMethod(preparedCommand);

            preparedParameters.Subject = selectedEntity;
            genericMethod.Invoke(null, new object[] { preparedParameters });
        }

        public void ExecuteCommand(CommandParameters parameters)
        {
            LaunchCommand(parameters);

            if (preparedParameters != null && preparedParameters.ExecuteOnce)
            {
                uiEvents.UnselectAllButtons();
                ResetCommand();
            }
        }
        void LaunchCommand(CommandParameters parameters)
        {
            if(preparedCommand == null) return;
            
            var createMethod = typeof(CommandFactory).GetMethod("Create", BindingFlags.Public | BindingFlags.Static);
            var genericMethod = createMethod.MakeGenericMethod(preparedCommand);
            
            CommandParameters.CombineParameters(parameters, preparedParameters);
            
            parameters.Subject = selectedEntity;
            var command = genericMethod.Invoke(null, new object[] { parameters });
            
            if(command != null) commandManager.ExecuteCommand((ICommand)command);
        }
        private void ResetCommand()
        {
            preparedCommand = null;
            preparedParameters = null;
            
            //levelEvents.SelectEntity(null);
        }
        public void ClearCommandStack()
        {
            ResetCommand();
            levelEvents.SelectEntity(null);
            commandManager.ClearStack();
        }

        
    }
}
