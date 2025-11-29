using System;
using System.Collections.Generic;
using CursedOnion.Game.Commands;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Events;
using CursedOnion.Game.Systems.Grid;
using Unit = CursedOnion.Game.Entity.Unit;

namespace CursedOnion.Game.Systems.Level
{
    public class LevelEvents : RuntimeEvents
    {
        #region GoldEvents
        public event Action<int> OnGoldUpdated;
        public void UpdateGold(int gold)
        {
            OnGoldUpdated?.Invoke(gold);
        }
        public event Action OnNotEnoughGold;
        public void InvokeNotEnoughGold()
        {
            OnNotEnoughGold?.Invoke();
        }
        #endregion

        #region SpawnEvents

        public event Action<int> OnUnitPlacedCountUpdated;
        public void UpdateUnitPlacedCount(int count)
        {
            OnUnitPlacedCountUpdated?.Invoke(count);
        }

        #endregion

        #region Selection Events

        public event Action<SimpleEntity> OnEntitySelected;
        public event Action OnNoEntitySelected;
        public void SelectEntity(SimpleEntity entity)
        {
            if(entity)
                OnEntitySelected?.Invoke(entity);
            else
                OnNoEntitySelected?.Invoke();
        }
        
        public event Action<StatData> OnStatDataSelected;
        public void SelectStatData(StatData data)
        {
            OnStatDataSelected?.Invoke(data);
        }
        
        public event Action OnTileSelectionRequested;
        public void RequestTileSelection()
        {
            OnTileSelectionRequested?.Invoke();
        }
        #endregion

        #region Command Events

        public event Action<Type, CommandParameters> OnCommandPrepareCalled;
        public void CallPrepareCommand<T>()
        {
            var commandType = typeof(T);
            OnCommandPrepareCalled?.Invoke(commandType, null);
        }
        public void CallPrepareCommand<T>(CommandParameters parameters)
        {
            var commandType = typeof(T);
            OnCommandPrepareCalled?.Invoke(commandType, parameters);
        }
        
        public event Action OnPreparedCommandCancelled;
        public void CancelPreparedCommand()
        {
            OnPreparedCommandCancelled?.Invoke();
        }

        #endregion

        #region Level Flow Events

        public event Action OnIntroCalled;
        public void CallIntro()
        {
            OnIntroCalled?.Invoke();
        }
        
        public event Action OnIntroFinished;
        public void InvokeIntroFinished()
        {
            OnIntroFinished?.Invoke();
        }
        public event Action<LevelState, LevelState> OnLevelStateChange;
        public void InvokeLevelState(LevelState previousState, LevelState newState)
        {
            OnLevelStateChange?.Invoke(previousState, newState);
            CancelPreparedCommand();
        }
        public event Action<bool> OnLevelCompleted;
        public void InvokeLevelCompleted(bool levelWon)
        {
            OnLevelCompleted?.Invoke(levelWon);
        }
        #endregion
        
        #region Turn Events
        public event Action<Unit> OnUnitTurnRegisterPetition;
        public void RegisterUnitForTurn(Unit unit)
        {
            OnUnitTurnRegisterPetition?.Invoke(unit);
        }
        
        public event Action<Unit> OnUnitTurnUnregisterPetition;
        public void UnregisterUnitForTurn(Unit unit)
        {
            OnUnitTurnUnregisterPetition?.Invoke(unit);
        }
        
        public event Action<List<Unit>> OnMergedUnitListUpdated;
        public void UpdateMergedUnitList(List<Unit> mergedUnits)
        {
            OnMergedUnitListUpdated?.Invoke(mergedUnits);
        }
        
        public event Action<bool> OnTurnBegin;
        public void InvokeTurnBegin(bool isPlayerTurn)
        {
            OnTurnBegin?.Invoke(isPlayerTurn);
        }
        
        public event Action OnTurnEnded;
        public void InvokeTurnEnd()
        {
            OnTurnEnded?.Invoke();
        }

        public event Action<SimpleEntity> OnTurnFocus;
        public void InvokeTurnFocus(SimpleEntity entity)
        {
            OnTurnFocus?.Invoke(entity);
        }
        #endregion

        #region Level Goal Events

        public event Action OnBossEnemyKilled;
        public void InvokeBossEnemyDeath()
        {
            OnBossEnemyKilled?.Invoke();
        }
        
        public event Action OnRoundPassed;
        public void PassRound()
        {
            OnRoundPassed?.Invoke();
        }
        
        public event Action OnAllEnemiesKilled;
        public void InvokeAllEnemiesDeath()
        {
            OnAllEnemiesKilled?.Invoke();
        }
        
        public event Action OnAllAlliesKilled;
        public void InvokeAllAlliesDeath()
        {
            OnAllAlliesKilled?.Invoke();
        }
        #endregion
        
        #region Graphic Events

        public event Action<bool> OnEnableBlackAndWhite;

        public void EnableBlackAndWhite(bool enable)
        {
            OnEnableBlackAndWhite?.Invoke(enable);
        }

        #endregion

    }
}