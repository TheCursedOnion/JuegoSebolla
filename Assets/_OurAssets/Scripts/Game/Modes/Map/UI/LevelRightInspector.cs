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
            levelName.SetKey(levelInformation.NameKey);
            levelDescription.SetKey(levelInformation.DescriptionKey);
        }
        
    }
}