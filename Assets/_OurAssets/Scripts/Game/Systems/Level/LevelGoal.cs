using System;
using CursedOnion.Game.Audio;
using CursedOnion.Locators;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Systems.Level.Goal
{
    public abstract class LevelGoal : MonoBehaviour
    {
        [Inject] protected RuntimeVariableLocator VariableLocator;
        [Inject] protected AudioGallery AudioGallery;
        [Inject] protected LevelEvents LevelEvents;
        [Inject] protected LevelManager LevelManager;
        protected abstract void OnEnable();
        protected abstract void OnDisable();
        protected virtual void CheckGoal() {}

        protected virtual void OnVictory()
        {
            LevelState nextState = LevelManager.LevelAsset.LevelData.LevelHasEndDialog ? LevelState.Finished : LevelState.InResults;
            if (!LevelManager.TrySetNewState(nextState)) return;
            
            if(nextState != LevelState.Finished) AudioGallery.StopAllMusic();
            
            LevelEvents.InvokeLevelCompleted(true);
            VariableLocator.SetCompletedLevel(LevelManager.LevelAsset.LevelData.LevelIndex);
        }

        protected virtual void OnDefeat()
        {
            if (!LevelManager.TrySetNewState(LevelState.InResults)) return;
            
            AudioGallery.StopAllMusic();
            LevelEvents.InvokeLevelCompleted(false);
        }

    }
}