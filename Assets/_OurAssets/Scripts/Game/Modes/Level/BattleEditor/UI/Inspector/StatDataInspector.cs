using System.Globalization;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Localization;
using CursedOnion.Game.Systems.Level;
using CursedOnion.Helpers;
using NaughtyAttributes;
using Reflex.Attributes;
using Reflex.Extensions;
using Reflex.Injectors;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CursedOnion.Game.Modes.Level.BattleEditor.UI
{
    public class StatDataInspector : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private GameObject statDataContainer;
        
        [HorizontalLine(height: 2f, color: EColor.Orange)]
        [SerializeField] private Image spritePreview;
        [SerializeField] private LocalizedGUIText entityName;
        
        [SerializeField] private StatLine hp;
        [SerializeField] private StatLine attack;
        [SerializeField] private StatLine defense;
        [SerializeField] private StatLine initiative;
        
        [SerializeField] private StatLine movementSpeed;
        [SerializeField] private StatLine priceValue;

        private SimpleEntity selectedEntity;
        private LevelEvents levelEvents;
        public void SetUp(LevelEvents levelEvents)
        {
            this.levelEvents = levelEvents;
            levelEvents.OnSpecialAbilitySelected += ProcessSpecialAbility;
            levelEvents.OnPreparedCommandCancelled += UpdateStats;
        }
        void OnDestroy()
        {
            if(levelEvents == null) return;
            
            levelEvents.OnSpecialAbilitySelected -= ProcessSpecialAbility;
            levelEvents.OnPreparedCommandCancelled -= UpdateStats;
        }
        void ProcessSpecialAbility(SimpleEntity entity, SpecialAbility ability)
        {
            if(entity != selectedEntity) return;

            string extraMessage = ability.ToString();
            StatFlag statFlag = ability.GetAffectedStats();
            
            if(string.IsNullOrEmpty(extraMessage) || statFlag.HasRaisedNone()) return;
            
            foreach (var flag in statFlag.GetActiveFlags())
            {
                switch (flag)
                {
                    case StatFlag.Health: hp.SetValue(StatLineParameters.Extra(extraMessage)); break;
                    case StatFlag.Damage: attack.SetValue(StatLineParameters.Extra(extraMessage)); break;
                    case StatFlag.Defense: defense.SetValue(StatLineParameters.Extra(extraMessage)); break;
                    case StatFlag.Initiative: initiative.SetValue(StatLineParameters.Extra(extraMessage)); break;
                    case StatFlag.Movement: movementSpeed.SetValue(StatLineParameters.Extra(extraMessage)); break;
                    case StatFlag.Price: priceValue.SetValue(StatLineParameters.Extra(extraMessage)); break;
                }
            }
        }
        
        public void ClearInspector()
        {
            EnableInspector(false);
        }

        public void ClearExtraTexts()
        {
            
        }
        void EnableInspector(bool enable)
        {
            statDataContainer.SetActive(enable);
            Color color = background.color;
            color.a = enable ? 0.8f : 0.3f;
            background.color = color;

            if (enable)
            {
                
            }
        }

        public void UpdateStatData(StatData data)
        {
            EnableInspector(true);

            spritePreview.sprite = data.InspectorSprite;
            entityName.SetKey(data.EntityNameKey);
            
            hp.SetValue(StatLineParameters.Value(data.Hp.ToString()));
            attack.SetValue(StatLineParameters.Value(data.Attack.ToString()));
            defense.SetValue(StatLineParameters.Value(data.Defense.ToString()));
            initiative.SetValue(StatLineParameters.Value(data.Initiative.ToString()));
            movementSpeed.SetValue(StatLineParameters.Value(data.Movement.ToString()));
            priceValue.SetValue(StatLineParameters.Value(data.Price.ToString()));
        }

        void UpdateStats()
        {
            UpdateStats(selectedEntity);
        }
        public void UpdateStats(SimpleEntity entity)
        {
            EnableInspector(true);
            
            RegisterForEntityUpdate(entity);
            
            var data = entity.StatData;
            var stats = entity.Stats;
            var statusHandler = entity.StatusHandler;
            
            spritePreview.sprite = data.InspectorSprite;
            entityName.SetKey(data.EntityNameKey);
            
            //TODO: ADDITIONAL EFFECTS
            Vector2Int currentHp = new Vector2Int(stats.CurrentHealthStat, stats.MaxHealthStat);
            hp.SetValue(StatLineParameters.Slider(currentHp));

            int attackStat = Mathf.CeilToInt(stats.AttackStat * statusHandler.AttackMultiplier);
            attack.SetValue(StatLineParameters.Value(attackStat.ToString()));
            
            defense.SetValue(StatLineParameters.Value(stats.DefenseStat.ToString()));
            
            initiative.SetValue(StatLineParameters.Value(stats.InitiativeStat.ToString()));
            
            int movementStat = stats.MovementStat + statusHandler.AdditionalMovement;
            movementSpeed.SetValue(StatLineParameters.Value(movementStat.ToString()));
            
            priceValue.SetValue(StatLineParameters.Value(stats.PriceStat.ToString()));
        }
        
        void RegisterForEntityUpdate(SimpleEntity entity)
        {
            if(selectedEntity == entity) return;
            
            if(selectedEntity != null) entity.OnEntityUpdate -= UpdateStats;
            selectedEntity = entity;
            selectedEntity.OnEntityUpdate += UpdateStats;
        }
    }
}
