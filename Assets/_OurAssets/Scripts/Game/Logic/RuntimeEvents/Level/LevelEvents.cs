using System;
using CursedOnion.Game.Commands;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Systems.Level;
using NaughtyAttributes;
using Reflex.Attributes;
using Unity.VisualScripting;
using UnityEngine;

namespace CursedOnion.Game.Events
{
    public class LevelEvents : RuntimeEvents
    {
        public event Action<int> OnGoldUpdated;
        public void UpdateGold(int gold)
        {
            OnGoldUpdated?.Invoke(gold);
        }
        
        public event Action<int> OnUnitPlacedCountUpdated;
        public void UpdateUnitPlacedCount(int count)
        {
            OnUnitPlacedCountUpdated?.Invoke(count);
        }
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
        

        LevelState currentLevelState;
        public event Action<LevelState, LevelState> OnLevelStateChange;
        public void SetNewLevelState(LevelState newState)
        {
            if(currentLevelState == newState) return;
            
            OnLevelStateChange?.Invoke(currentLevelState, newState);
            currentLevelState = newState;
            
            CancelPreparedCommand();
        }
        
    }
}