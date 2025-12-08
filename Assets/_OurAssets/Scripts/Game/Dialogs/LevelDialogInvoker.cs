using System;
using CursedOnion.Game.Logic.Services;
using CursedOnion.Game.Systems.Level;
using CursedOnion.Locators;
using Reflex.Attributes;
using Reflex.Extensions;
using UnityEngine;

namespace CursedOnion.Game.Dialog
{
    public class LevelDialogInvoker : DialogInvoker
    {
        [Inject] LevelEvents levelEvents;
        
        public DialogBlock StartingDialogBlock;
        public DialogBlock EndDialogBlock;
        
        DialogController dialogController;
        public void Start()
        {
            levelEvents.OnLevelStateChange += TryPlayEndDialog;
            RequestDialog(StartingDialogBlock);
        }

        private void OnDestroy()
        {
            levelEvents.OnLevelStateChange -= TryPlayEndDialog;
        }

        void TryPlayEndDialog(LevelState _, LevelState newState)
        {
            if (newState == LevelState.Finished) RequestDialog(EndDialogBlock);
        }
    }
}