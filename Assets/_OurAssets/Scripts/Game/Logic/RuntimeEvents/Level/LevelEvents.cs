using System;
using CursedOnion.Game.Commands;
using CursedOnion.Game.Entity;
using Reflex.Attributes;

namespace CursedOnion.Game.Events
{
    public class LevelEvents : RuntimeEvents
    {
        
        public event Action<SimpleEntity> OnEntitySelected;
        public event Action OnNoEntitySelected;
        public void SelectEntity(SimpleEntity entity)
        {
            if(entity)
                OnEntitySelected?.Invoke(entity);
            else
                OnNoEntitySelected?.Invoke();
        }
        
        public event Action<Type, CommandParameters> OnCommandPrepareCalled;
        public void CallPrepareCommand<T>() where T : ICommand
        {
            var commandType = typeof(T);
            OnCommandPrepareCalled?.Invoke(commandType, null);
        }
        public void CallPrepareCommand<T>(CommandParameters parameters) where T : ICommand
        {
            var commandType = typeof(T);
            OnCommandPrepareCalled?.Invoke(commandType, parameters);
        }
        
        public event Action OnPreparedCommandCancelled;
        public void CancelPreparedCommand()
        {
            OnPreparedCommandCancelled?.Invoke();
        }
        
        public event Action OnLevelBattleStart;
        public void StartBattle()
        {
            OnLevelBattleStart?.Invoke();
        }
        
    }
}