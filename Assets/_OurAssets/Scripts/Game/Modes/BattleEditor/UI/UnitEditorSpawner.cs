using System;
using System.Linq;
using CursedOnion.Game.Commands;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Events;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Modes.Editor.UI
{
    public class UnitEditorSpawner : MonoBehaviour
    {
        [Inject] LevelEvents levelEvents;
        
        GameObject selectedUnit;
        private CommandParameters spawnParameters;

        private void Awake()
        {
            CommandParameters.Builder builder = new CommandParameters.Builder();
            spawnParameters = builder.Build();
        }

        private void OnEnable()
        {
            levelEvents.OnNoEntitySelected += UnselectUnit;
        }
        private void OnDisable()
        {
            levelEvents.OnNoEntitySelected -= UnselectUnit;
        }

        public void ToggleSelectForSpawn(GameObject unitPrefab)
        {
            if (selectedUnit != null && selectedUnit == unitPrefab)
            {
                levelEvents.SelectEntity(null);
                levelEvents.CancelPreparedCommand();
                return;
            }
            
            selectedUnit = unitPrefab;
            levelEvents.SelectEntity(unitPrefab.GetComponent<Unit>());
            
            spawnParameters.EntityPrefab = selectedUnit;
            levelEvents.CallPrepareCommand<SpawnCommand>(spawnParameters);
        }
        void UnselectUnit()
        {
            selectedUnit = null;
        }
    }
}
