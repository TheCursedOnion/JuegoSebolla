using System;
using CursedOnion.Game.Commands;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Systems.Level;
using Reflex.Attributes;
using Unity.VisualScripting;
using UnityEngine;

namespace CursedOnion.Game.Events
{
    public class LevelEvents : RuntimeEvents
    {
        
        public LevelEvents(LevelData levelData)
        {
            remainingGold = levelData.StartingGold;
        }

        public void InvokeInitialCalls()
        {
            AddGold(0);
            SelectEntity(null);
            
            InvokeLevelState(currentLevelState, false);
        }

        private int remainingGold;
        public int RemainingGold => remainingGold;
        public event Action<int> OnGoldUpdated;
        public bool AddGold(int gold)
        {
            if (gold < 0)
            {
                return false;
            }
            return ModifyGold(gold);
        }
        public bool TakeGold(int gold)
        {
            if (gold < 0 || remainingGold < gold)
            {
                return false;
            }
            return ModifyGold(-gold);
        }
        private bool ModifyGold(int amount)
        {
            remainingGold += amount;
            Debug.Log($"GOLD: {remainingGold}");
            OnGoldUpdated?.Invoke(remainingGold);
            return true;
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
        
        public event Action OnLevelDialogStart;
        public event Action OnLevelDialogFinish;
        
        public event Action OnLevelBattleEditorStart;
        public event Action OnLevelBattleEditorFinish;
        
        public event Action OnLevelBattleStart;
        public event Action OnLevelBattleFinish;
        public void SetLevelState(LevelState state)
        {
            if(currentLevelState == state) return;
            InvokeLevelState(state, true);
            currentLevelState = state;
            InvokeLevelState(state, false);
        }
        
        void InvokeLevelState(LevelState state, bool finish)
        {
            switch (state)
            {
                case LevelState.InDialog:
                        if(finish) OnLevelDialogFinish?.Invoke();
                        else OnLevelDialogStart?.Invoke();
                    break;
                case LevelState.InBattleEditor:
                        if(finish) OnLevelBattleEditorFinish?.Invoke();
                        else OnLevelBattleEditorStart?.Invoke();
                    break;
                case LevelState.InBattle:
                    if(finish) OnLevelBattleFinish?.Invoke();
                    else OnLevelBattleStart?.Invoke();
                break;
            }
        }
        
    }
}