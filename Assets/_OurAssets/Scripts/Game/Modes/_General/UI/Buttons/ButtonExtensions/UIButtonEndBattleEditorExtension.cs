using CursedOnion.Game.Events;
using CursedOnion.Game.General.UI.Buttons;
using CursedOnion.Game.Systems.Level;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.General.UI.Buttons.Extensions
{
    public class UIButtonEndBattleEditorExtension : MonoBehaviour
    {
        [Inject] LevelManager levelManager;
        UIButton uiButton;
        void Awake()
        {
            uiButton = GetComponent<UIButton>();
        }
        protected virtual void OnEnable()
        {
            levelManager.LevelEvents.OnUnitPlacedCountUpdated += CheckPlacedUnits;
            CheckPlacedUnits(levelManager.LevelScoreVariables.PlacedUnits);
        }
        protected virtual void OnDisable()
        {
            levelManager.LevelEvents.OnUnitPlacedCountUpdated -= CheckPlacedUnits;
        }
        void CheckPlacedUnits(int unitCount)
        {
            bool canBeInteracted = unitCount > 0;
            uiButton.SetInterative(canBeInteracted);
        }
        
    }
}
