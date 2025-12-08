using CursedOnion.Game.Entity;
using CursedOnion.Game.Localization;
using CursedOnion.Game.Systems.Level;
using CursedOnion.Helpers;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace CursedOnion.Game.Modes.Level.BattleEditor.UI
{
    public class EntityInspector : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private Image effectsBackground;
        [SerializeField] private Color allyColor;
        [SerializeField] private Color enemyColor;
        [SerializeField] private Color neutralColor;
        
        [SerializeField] private GameObject statDataContainer;
        
        [HorizontalLine(height: 2f, color: EColor.Orange)]
        [SerializeField] private Image spritePreview;
        [SerializeField] private LocalizedGUIText entityName;
        
        [HorizontalLine(height: 2f, color: EColor.Orange)]
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
        
        void EnableInspector(bool enable)
        {
            statDataContainer.SetActive(enable);
            Color color = background.color;
            color.a = enable ? 0.8f : 0.3f;
            
            background.color = color;
            effectsBackground.color = color;
        }

        void SetBackgroundColor(Color color)
        {
            Color c = background.color;
            color.a = c.a;
            background.color = color;
            effectsBackground.color = color;
        }
        public void UpdateStatData(StatData data)
        {
            EnableInspector(true);
            SetBackgroundColor(neutralColor);
            
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
            if(selectedEntity != null && !selectedEntity.ActionHandler.HasDied())
                UpdateStats(selectedEntity);
            else
                ClearInspector();
        }
        public void UpdateStats(SimpleEntity entity)
        {
            if (entity == null || entity.ActionHandler.HasDied())
            {
                ClearInspector();
                return;
            }
            
            EnableInspector(true);

            Color backgroundColor = entity.GetSide() switch
            {
                BattleSide.Ally => allyColor,
                BattleSide.Enemy => enemyColor,
                _ => neutralColor
            };
            SetBackgroundColor(backgroundColor);
            
            RegisterForEntityUpdate(entity);
            
            var data = entity.StatData;
            var stats = entity.Stats;
            var statusHandler = entity.StatusHandler;
            
            spritePreview.sprite = data.InspectorSprite;
            entityName.SetKey(data.EntityNameKey);
            
            
            Vector2Int currentHp = new Vector2Int(stats.CurrentHealthStat, stats.MaxHealthStat);
            Vector2Int currentShield = new Vector2Int(statusHandler.AdditionalHP, statusHandler.MaxAdditionalHP);
            hp.SetValue(StatLineParameters.Slider(currentHp, currentShield, statusHandler.HasAdditionalHP()));

            int attackStat = Mathf.CeilToInt(stats.AttackStat * statusHandler.AttackMultiplier);
            attack.SetValue(StatLineParameters.Value(attackStat.ToString(), statusHandler.HasAttackMultiplier()));
            
            defense.SetValue(StatLineParameters.Value(stats.DefenseStat.ToString(), statusHandler.HasAdditionalHP()));
            
            initiative.SetValue(StatLineParameters.Value(stats.InitiativeStat.ToString()));
            
            int movementStat = stats.MovementStat + statusHandler.AdditionalMovement;
            movementSpeed.SetValue(StatLineParameters.Value(movementStat.ToString(), statusHandler.HasAdditionalMovement()));
            
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
