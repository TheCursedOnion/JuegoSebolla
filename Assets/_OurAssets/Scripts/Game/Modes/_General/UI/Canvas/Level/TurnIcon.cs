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
        
        LevelManager levelManager;
        LevelEvents levelEvents;
        TurnSystem turnSystem;
        
        TurnInspector inspector;
        private bool canRequestScroll = false;
        public void Initialize(LevelManager levelManager, TurnInspector inspector)
        {
            this.inspector = inspector;
            this.levelManager = levelManager;
            this.levelEvents = levelManager.LevelEvents;
            turnSystem = this.levelManager.GetTurnSystem();
            
            this.levelEvents.OnTurnEnded += UnhighlightInterior;
        }
        private void OnDestroy()
        {
            levelEvents.OnTurnEnded -= UnhighlightInterior;
            if (unit != null)
            {
                unit.OnStartTurn -= HighlightInterior;
            }
        }

        public void EnableCanRequestScroll(bool enable) => canRequestScroll = enable;
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

            Color color = inactiveColor;
            
            if(unit.HasTurn)
                color = unit.GetSide() == BattleSide.Ally ? allyHighlightColor : enemyHighlightColor;
           
            SetImageColor(interior, color);
            
            unit.OnStartTurn += HighlightInterior;
            icon.sprite = newUnit.StatData.InspectorSprite;
        }
        void ColorBorder(Unit unit)
        {
            Color color = unit.GetSide() == BattleSide.Ally ? allyColor : enemyColor;
            SetImageColor(border, color);
        }

        public void ResetHighlight()
        {
            if(unit == null) return;

            Color color = inactiveColor;

            var turnInfo = turnSystem.GetCurrentTurnInformation();
            BattleSide side = turnInfo.Item1 ? BattleSide.Enemy : BattleSide.Ally;
            
            if(unit.GetSide() == side && turnInfo.Item2 == unit.Stats.InitiativeStat)
                color = unit.GetSide() == BattleSide.Ally ? allyHighlightColor : enemyHighlightColor;
            
            SetImageColor(interior, color);
        }
        
        void HighlightInterior()
        {
            if(highlighted) return;
            
            highlighted = true;
            
            if(canRequestScroll)
                inspector.FocusOnIcon(this);
            
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