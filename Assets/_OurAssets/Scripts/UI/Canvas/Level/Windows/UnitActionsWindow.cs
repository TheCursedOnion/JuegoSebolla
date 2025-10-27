using System.Collections.Generic;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Entity.UI;
using UnityEngine;

namespace CursedOnion
{
    public class UnitActionsWindow : MonoBehaviour
    {
        private Dictionary<GameObject, GameObject> actionsUI = new();
        
        private Unit unit;
        public Unit Unit => unit;
        public void SetEntity(Unit unit)
        {
            this.unit = unit;
            UpdateActionsWindow();
        }

        void UpdateActionsWindow()
        {
            DisableChildren();
            
            if(unit == null) return;
            
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
