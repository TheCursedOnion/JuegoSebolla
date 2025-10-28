using System;
using CursedOnion.Game.Commands;
using CursedOnion.Game.Entity;
using Reflex.Attributes;

namespace CursedOnion.Game.Events
{
    public class LevelEvents : RuntimeEvents
    {
        
        public event Action<SimpleEntity> OnEntitySelected;
        public void SelectEntity(SimpleEntity entity)
        {
            OnEntitySelected?.Invoke(entity);
        }
        
        public event Action<Type> OnCommandPrepareCalled;
        public void CallPrepareCommand<T>() where T : EntityCommand
        {
            var commandType = typeof(T);
            OnCommandPrepareCalled?.Invoke(commandType);
        }
        
    }
}