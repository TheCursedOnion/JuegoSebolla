using System;
using CursedOnion.Game.Events;
using CursedOnion.Game.General.UI.Buttons;
using CursedOnion.Game.Systems.Level;
using Reflex.Attributes;
using Reflex.Extensions;
using UnityEngine;

namespace CursedOnion.Game.General.UI.Buttons.Extensions
{
    public class UIButtonEndBattleEditorExtension : MonoBehaviour
    {
        [Inject] LevelManager levelManager;
        UIButton uiButton;
        void Awake()
        {
            levelManager ??= gameObject.scene.GetSceneContainer().Resolve<LevelManager>();
            uiButton = GetComponent<UIButton>();
        }

        private void Start()
        {
            CheckPlacedUnits(levelManager.LevelScoreVariables.PlacedUnits);
        }

        protected virtual void OnEnable()
        {
            levelManager.LevelEvents.OnUnitPlacedCountUpdated += CheckPlacedUnits;
        }
        protected virtual void OnDisable()
        {
            levelManager.LevelEvents.OnUnitPlacedCountUpdated -= CheckPlacedUnits;
        }
        void CheckPlacedUnits(int unitCount)
        {
            bool canBeInteracted = unitCount > 0;
            uiButton.SetInteractive(canBeInteracted);
        }
        
    }
}
