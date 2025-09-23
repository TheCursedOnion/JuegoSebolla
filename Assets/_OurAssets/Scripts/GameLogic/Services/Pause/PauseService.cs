using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CursedOnion.GameLogic.Services.Pause
{
    public class PauseService : IService
    { 
        public Action<PauseLevel> OnPauseUpdate;
        
        public PauseLevel PauseLevel => (PauseLevel) pauseStack.Peek();
        private Stack<int> pauseStack = new Stack<int>(new[] {(int)PauseLevel.None});

        public void Pause(PauseLevel pauseLevel)
        {
            int nextPauseLevel = (int)pauseLevel;
            if(pauseStack.Peek() >= nextPauseLevel) return;
            
            PushIntoPauseStack(nextPauseLevel);
        }
        public void PauseAll()
        {
            int maxPauseLevel = Enum.GetValues(typeof(PauseLevel)).Length - 1;
            PushIntoPauseStack(maxPauseLevel);
        }
        void PushIntoPauseStack(int pauseLevel)
        {
            pauseStack.Push(pauseLevel);
            InvokeUpdate((PauseLevel)pauseLevel);
        }



        public void Unpause(PauseLevel pauseLevel)
        {
            int pauseLevelToDrop = (int)pauseLevel;
            if(pauseStack.Peek() != pauseLevelToDrop) return;

            UnpauseCurrentLevel();
        }
        public void UnpauseCurrentLevel()
        {
            pauseStack.Pop();
            InvokeUpdate((PauseLevel)pauseStack.Peek());
        }
        public void UnpauseAll()
        {
            while (pauseStack.Peek() != 0)
            {
                pauseStack.Pop();
            }
            InvokeUpdate(0);
        }

        private void InvokeUpdate(PauseLevel pauseLevel)
        {
            Debug.LogWarning($"Current Pause Level {pauseLevel}");
            OnPauseUpdate?.Invoke(pauseLevel);
        }
        
    }
}
