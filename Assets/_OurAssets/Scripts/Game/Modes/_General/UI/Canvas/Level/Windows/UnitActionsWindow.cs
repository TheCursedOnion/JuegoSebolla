using System.Collections.Generic;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Entity.UI;
using CursedOnion.Game.Events;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion
{
    public class UnitActionsWindow : MonoBehaviour
    {
        [Inject] LevelEvents levelEvents;
        private Dictionary<GameObject, GameObject> actionsUI = new();
        
        private Unit unit;
        public Unit Unit => unit;
        private void OnEnable()
        {
            levelEvents.OnEntitySelected += SetEntity;
            levelEvents.OnNoEntitySelected += SetNullEntity;
        }
        private void OnDisable()
        {
            levelEvents.OnEntitySelected += SetEntity;
            levelEvents.OnNoEntitySelected += SetNullEntity;
        }
        
        void SetNullEntity()
        {
            unit = null;
            DisableChildren();
        }
        void SetEntity(SimpleEntity entity)
        {
            unit = entity as Unit;
            UpdateActionsWindow();
        }
        void UpdateActionsWindow()
        {
            var ui = unit.GetUI();
            GameObject instancedUI;
            
            if (!actionsUI.TryGetValue(ui, out instancedUI))
            {
                instancedUI = GameObject.Instantiate(ui, transform);
                instancedUI.GetComponent<UnitUI>().Initialize();
                instancedUI.name = ui.name;
                actionsUI.Add(ui, instancedUI);
            }
            instancedUI.GetComponent<UnitUI>().AssociateUnit(unit);
            instancedUI.SetActive(true);
        }
        void DisableChildren()
        {
            foreach (var action in actionsUI)
            {
                action.Value.SetActive(false);
            }
        }
    }
}
