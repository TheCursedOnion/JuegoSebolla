using CursedOnion.Game.Entity;
using CursedOnion.Game.Modes.Level.BattleEditor.UI;
using CursedOnion.Game.Systems.Level;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.General.UI.Canvases.Level
{
    public class UnitInformationWindow : MonoBehaviour
    {
        [Inject] LevelEvents levelEvents;
        
        [SerializeField] EntityInspector entityInspector;
        [SerializeField] EffectInspector effectInspector;
        public void Initialize(LevelManager levelManager)
        {
            levelEvents = levelManager.LevelEvents;
            entityInspector.SetUp(levelEvents);
            effectInspector.SetUp();
            ClearInspectors();
            OnEnable();
        }
        private void OnEnable()
        {
            if (levelEvents == null) return;
            OnDisable();
            
            levelEvents.OnStatDataSelected += UpdateInspectedData;
            levelEvents.OnEntitySelected += UpdateInspectedStats;
            levelEvents.OnNoEntitySelected += ClearInspectors;
        }
        private void OnDisable()
        {
            levelEvents.OnStatDataSelected -= UpdateInspectedData;
            levelEvents.OnEntitySelected -= UpdateInspectedStats;
            levelEvents.OnNoEntitySelected -= ClearInspectors;
        }
        void ClearInspectors()
        {
            entityInspector.ClearInspector();
            effectInspector.ClearInspector();
        }
        void UpdateInspectedData(StatData data)
        {
            if (data == null)
            {
                ClearInspectors();
                levelEvents.RequestTileSelection();
            }
            else
            {
                entityInspector.UpdateStatData(data);
                effectInspector.ClearInspector();
            }
        }

        void UpdateInspectedStats(SimpleEntity entity)
        {
            entityInspector.UpdateStats(entity);
            effectInspector.UpdateEffects(entity);
        }
    }
}