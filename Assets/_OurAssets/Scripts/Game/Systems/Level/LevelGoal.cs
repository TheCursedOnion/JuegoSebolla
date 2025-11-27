using System;
using CursedOnion.Locators;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Systems.Level.Goal
{
    public abstract class LevelGoal : MonoBehaviour
    {
        [Inject] protected RuntimeVariableLocator VariableLocator;
        [Inject] protected LevelEvents LevelEvents;
        [Inject] protected LevelManager LevelManager;
        protected abstract void OnEnable();
        protected abstract void OnDisable();
        protected virtual void CheckGoal() {}

        protected virtual void OnVictory()
        {
            if (!LevelManager.TrySetNewState(LevelState.Finished)) return;
            Debug.Log("Victoria");
        }

        protected virtual void OnDefeat()
        {
            if (!LevelManager.TrySetNewState(LevelState.Finished)) return;
            Debug.Log("Derrota");
        }

    }
}