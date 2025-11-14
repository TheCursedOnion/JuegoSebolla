using System;
using CursedOnion.Game.Entity;
using CursedOnion.Game.General.UI.Buttons;
using UnityEngine;
using UnityEngine.UI;

namespace CursedOnion.Game.Modes.Level.BattleEditor.UI
{
    public class UnitButtonSpawner : MonoBehaviour
    {
        [SerializeField] private Image buttonImage;
        [SerializeField] private UnitEditorSpawner spawner;
        
        [SerializeField] private StatData statData;
        
        private void Awake()
        {
            buttonImage.sprite = statData.InspectorSprite;
        }

        public StatData GetUnitStats() => statData;
        
        public void SpawnUnit()
        {
            spawner.ToggleSelectForSpawn(this);
        }
    }
}
