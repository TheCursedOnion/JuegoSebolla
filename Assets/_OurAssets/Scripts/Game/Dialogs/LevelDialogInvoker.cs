using System;
using CursedOnion.Game.Logic.Services;
using CursedOnion.Game.Systems.Level;
using CursedOnion.Locators;
using Reflex.Attributes;
using Reflex.Extensions;
using UnityEngine;

namespace CursedOnion.Game.Dialog
{
    public class LevelDialogInvoker : MonoBehaviour
    {
        [Inject] RuntimeVariableLocator variableLocator;
        [Inject] LevelEvents levelEvents;
        
        public DialogBlock StartingDialogBlock;
        public DialogBlock EndDialogBlock;
        
        DialogController dialogController;
        public void Start()
        {
            levelEvents.OnLevelStateChange += TryPlayEndDialog;
            dialogController = variableLocator.GetDialogController();
            
            if(StartingDialogBlock != null && !string.IsNullOrEmpty(StartingDialogBlock.Name))
                dialogController.PlayDialog(StartingDialogBlock, gameObject.scene.GetSceneContainer());
        }

        private void OnDestroy()
        {
            levelEvents.OnLevelStateChange -= TryPlayEndDialog;
        }

        void TryPlayEndDialog(LevelState _, LevelState newState)
        {
            if(newState == LevelState.Finished) dialogController?.PlayDialog(EndDialogBlock, gameObject.scene.GetSceneContainer());
        }
    }
}