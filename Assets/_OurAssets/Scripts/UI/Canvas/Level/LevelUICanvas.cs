using System;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Events;
using CursedOnion.Game.Systems.Level;
using CursedOnion.ScriptableObjects;
using CursedOnion.UI;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.UI.Canvases.Level
{
    public class LevelUICanvas : MonoBehaviour, IUICanvas
    {
        [Inject] LevelEvents levelEvents;
        [SerializeField] private UnitActionsWindow actionsWindow;
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
            actionsWindow.SetEntity(null);
        }
        void SetEntity(SimpleEntity entity)
        {
            if(entity is Unit unit) actionsWindow.SetEntity(unit);
        }
    }
}