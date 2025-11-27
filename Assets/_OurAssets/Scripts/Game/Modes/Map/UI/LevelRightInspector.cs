using CursedOnion.Game.Events;
using CursedOnion.Game.Localization;
using CursedOnion.Game.Objects;
using CursedOnion.Locators;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Modes.Map.UI
{
    public class LevelRightInspector : MonoBehaviour
    {
        [Inject] MapEvents mapEvents;
        
        [SerializeField] LocalizedGUIText levelName;
        [SerializeField] LocalizedGUIText levelDescription;
        private void Awake()
        {
            mapEvents.OnLevelSelected += ProcessLevelSelected;
        }
        private void OnDestroy()
        {
            mapEvents.OnLevelSelected -= ProcessLevelSelected;
        }
        void ProcessLevelSelected(LevelInformation levelInformation)
        {
            string nameKey = levelInformation.BaseKey;
            string goalKey = levelInformation.BaseKey + "_goal";
            string descKey = levelInformation.BaseKey + "_desc";
            
            levelName.SetKey(nameKey);
            levelDescription.SetKey(descKey);
        }
        
    }
}