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
        private Type preparedCommand;
        public EntityCommandHandler(Container sceneContainer)
        {
            AttributeInjector.Inject(this, sceneContainer);
            
            levelEvents.OnEntitySelected += SelectEntity;
            levelEvents.OnCommandPrepareCalled += PrepareEntityCommand;
        }
        public void Dispose()
        {
            levelEvents.OnEntitySelected -= SelectEntity;
            levelEvents.OnCommandPrepareCalled -= PrepareEntityCommand;
        }
        
        public void SelectEntity(SimpleEntity entity)
        {
            if(entity == null) return;
            
            if(!IsValidCommandSubject(entity)) ResetCommand(true);
            else if(entity != selectedEntity) commandManager.ClearStack();
            
            selectedEntity = entity;
        }
        bool IsValidCommandSubject(SimpleEntity entity)
        {
            var commandableEntity = entity as CommandableEntity;
            return commandableEntity is not null;
        }
        
        
        public bool HasPreparedCommand()
        {
            return preparedCommand != null && IsValidCommandSubject(selectedEntity);
        }
        
        void PrepareEntityCommand(Type commandType)
        {
            if(selectedEntity == null) return;
            preparedCommand = commandType;
        }
        
        public void LaunchCommand(EntityCommandParameters parameters)
        {
            if(!TryGetCommandSubject(out var commandSubject)) return;
            
            var createMethod = typeof(EntityCommandFactory).GetMethod("Create", BindingFlags.Public | BindingFlags.Static);
            var genericMethod = createMethod.MakeGenericMethod(preparedCommand);

            var command = genericMethod.Invoke(null, new object[] { commandSubject, parameters });

            commandManager.ExecuteCommand((ICommand)command);
            ResetCommand(false);
        }
        bool TryGetCommandSubject(out CommandableEntity commandableEntity)
        {
            commandableEntity = selectedEntity as CommandableEntity;
            return commandableEntity is not null;
        }
        private void ResetCommand(bool resetSelectedEntity)
        {
            preparedCommand = null;
            
            if(resetSelectedEntity) selectedEntity = null;
            //SelectEntity(null);
        }
        
        public void ClearCommandStack()
        {
            commandManager.ClearStack();
        }

        
    }
}
