using CursedOnion.Game.Entity;
using CursedOnion.Game.Localization;
using NaughtyAttributes;
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
        }

        public void SetInspectorStatData(StatData data)
        {
            EnableInspector(true);

            spritePreview.sprite = data.InspectorSprite;
            entityName.SetKey(data.EntityNameKey);
                
            hp.SetRangedValue(data.Hp.Range);
            attack.SetRangedValue(data.Attack.Range);
            defense.SetRangedValue(data.Defense.Range);
            initiative.SetRangedValue(data.Initiative.Range);
            
            movementSpeed.SetValue(data.Movement);
            priceValue.SetValue(data.Price);
        }
        public void SetInspectorStats(SimpleEntity entity)
        {
            EnableInspector(true);
            
            RegisterForEntityUpdate(entity);
            
            var data = entity.StatData;
            var stats = entity.Stats;
            
            spritePreview.sprite = data.InspectorSprite;
            entityName.SetKey(data.EntityNameKey);
            
            hp.SetValue($"{stats.CurrentHealthStat}/{stats.MaxHealthStat}");
            
            attack.SetValue(stats.AttackStat);
            defense.SetValue(stats.DefenseStat);
            initiative.SetValue(stats.InitiativeStat);
            
            movementSpeed.SetValue(stats.MovementStat);
            priceValue.SetValue(stats.PriceStat);
        }

        void RegisterForEntityUpdate(SimpleEntity entity)
        {
            if(selectedEntity != null) entity.OnEntityUpdate -= SetInspectorStats;
            
            selectedEntity = entity;
            selectedEntity.OnEntityUpdate += SetInspectorStats;
        }
    }
}
