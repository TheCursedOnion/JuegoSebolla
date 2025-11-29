using System;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Systems.Level;
using Reflex.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace CursedOnion.Game.Modes.Level.Battle.UI
{
    public class TurnIcon : MonoBehaviour
    {
        [SerializeField] private Image border;
        [SerializeField] private Image interior;
        [SerializeField] private Image icon;
        
        [Header("\nColors")]
        [SerializeField] private Color allyColor;
        [SerializeField] private Color enemyColor;
        
        [SerializeField] private Color allyHighlightColor;
        [SerializeField] private Color enemyHighlightColor;
        
        [SerializeField] private Color inactiveColor;
        
        Unit unit;
        bool highlighted = false;
        LevelEvents levelEvents;
        private void Awake()
        {
            levelEvents = gameObject.scene.GetSceneContainer().Resolve<LevelEvents>();
            levelEvents.OnTurnEnded += UnhighlightInterior;
        }

        private void OnDestroy()
        {
            levelEvents.OnTurnEnded -= UnhighlightInterior;
            if(unit != null) unit.OnStartTurn -= HighlightInterior;
        }

        public void CheckUnit()
        {
            unit?.CheckUnit();
        }
        public void AssignUnit(Unit newUnit)
        {
            if(unit == newUnit) return;
            
            if(unit != null) unit.OnStartTurn -= HighlightInterior;
            
            highlighted = false;
            
            unit = newUnit;
            ColorBorder(unit);
            SetImageColor(interior, inactiveColor);
            
            unit.OnStartTurn += HighlightInterior;
            icon.sprite = newUnit.StatData.InspectorSprite;
        }
        void ColorBorder(Unit unit)
        {
            Color color = unit.GetSide() == BattleSide.Ally ? allyColor : enemyColor;
            SetImageColor(border, color);
        }
        
        void HighlightInterior()
        {
            if(highlighted) return;
            
            highlighted = true;
            Color color = unit.GetSide() == BattleSide.Ally ? allyHighlightColor : enemyHighlightColor;
            SetImageColor(interior, color);
        }
        void UnhighlightInterior()
        {
            if(!highlighted) return;

            highlighted = false;
            SetImageColor(interior, inactiveColor);
        }
        void SetImageColor(Image image, Color color)
        {
            image.color = color;
        }
    }
}