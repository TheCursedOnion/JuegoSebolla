using System;
using CursedOnion.Game.Entity;
using CursedOnion.ScriptableObjects;
using CursedOnion.UI;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.UI.Canvases.Level
{
    public class LevelUICanvas : MonoBehaviour, IUICanvas
    {
        [Inject] LevelManager levelManager;
        [SerializeField] private UnitActionsWindow actionsWindow;
        
        private SimpleEntity inspectedEntity;
        public SimpleEntity InspectedEntity => inspectedEntity;
        
        private void Awake()
        {
            levelManager.CommandHandler.OnEntitySelected += UpdateWindows;
        }

        void UpdateWindows(SimpleEntity inspectedEntity)
        {
            this.inspectedEntity = inspectedEntity;
            if (inspectedEntity == null)
            {
                SetNullEntity();
            }
            else
            {
                SetEntity(inspectedEntity);
            }
        }
        void SetNullEntity()
        {
            actionsWindow.SetEntity(null);
        }
        void SetEntity(SimpleEntity entity)
        {
            if(entity is Unit unit) actionsWindow.SetEntity(unit);
        }
    }
}