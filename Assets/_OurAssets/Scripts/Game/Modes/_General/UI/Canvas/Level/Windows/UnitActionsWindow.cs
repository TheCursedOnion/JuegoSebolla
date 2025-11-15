using System.Collections.Generic;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Entity.UI;
using CursedOnion.Game.Events;
using CursedOnion.Game.General.UI.Buttons;
using CursedOnion.Game.Systems.Level;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace CursedOnion
{
    public class UnitActionsWindow : MonoBehaviour
    {
        [SerializeField] private Image actionsImageBackground;
        [SerializeField] private UIButton turnButton;
        
        LevelManager levelManager;
        LevelEvents levelEvents;
        TurnSystem turnSystem;
        
        private Dictionary<GameObject, GameObject> actionsUI = new();
        private Unit unit;
        
        public void Initialize(LevelManager levelManager)
        {
            this.levelManager = levelManager;
            levelEvents = levelManager.LevelEvents;
            turnSystem = levelManager.GetTurnSystem();
            OnEnable();
        }
        private void OnEnable()
        {
            if (levelEvents == null) return;
            OnDisable();
            levelEvents.OnEntitySelected += SetEntity;
            levelEvents.OnNoEntitySelected += SetNullEntity;
            levelEvents.OnTurnBegin += OnTurnChanged;
        }

        private void OnDisable()
        {
            if (levelEvents == null) return;
            levelEvents.OnEntitySelected -= SetEntity;
            levelEvents.OnNoEntitySelected -= SetNullEntity;
            levelEvents.OnTurnBegin -= OnTurnChanged;
        }

        private void OnTurnChanged(bool isPlayerTurn)
        {
            //if (unit != null) UpdateActionsWindow();
            turnButton.SetInteractive(isPlayerTurn);
        }

        void SetNullEntity()
        {
            ShowActionsWindow(false);
            DisableChildren();
            unit = null;
        }
        void SetEntity(SimpleEntity entity)
        {
            DisableChildren();
            
            unit = entity as Unit;
            bool hasTurn = unit != null && unit.GetSide() != BattleSide.Enemy && turnSystem.IsUnitActive(unit);
            
            ShowActionsWindow(hasTurn);
            if (hasTurn) UpdateActionsWindow();
        }
        void ShowActionsWindow(bool show)
        {
            Color color = actionsImageBackground.color;
            color.a = show ? 0.7f : 0;
            actionsImageBackground.color = color;
        }
        void UpdateActionsWindow()
        {
            var ui = unit.GetUI();
            GetCorrespondingUI(ui);
        }
        void GetCorrespondingUI(GameObject uiKey)
        {
            if (!actionsUI.TryGetValue(uiKey, out var instancedUI))
            {
                instancedUI = Instantiate(uiKey, transform);
                instancedUI.GetComponent<UnitUI>().Initialize();
                instancedUI.name = uiKey.name;
                actionsUI.Add(uiKey, instancedUI);
            }
            ConfigureUI(instancedUI.GetComponent<UnitUI>(), unit);
        }
        void ConfigureUI(UnitUI ui, Unit unit)
        {
            ui.AssociateUnit(unit);
            ui.gameObject.SetActive(true);
        }
        void DisableChildren()
        {
            foreach (var ui in actionsUI)
            {
                ui.Value.SetActive(false);
            }
        }
        
    }
}
