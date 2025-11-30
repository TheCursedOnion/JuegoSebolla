using System;
using System.Collections.Generic;
using System.Linq;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Entity.Effects;
using CursedOnion.Helpers;
using UnityEngine;
using UnityEngine.Pool;

namespace CursedOnion.Game.General.UI.Canvases.Level
{
    public class EffectInspector : MonoBehaviour
    {
        [SerializeField] private GameObject effectIconPrefab;
        [SerializeField] private Transform effectIconContainer;
        
        private SimpleEntity selectedEntity;
        private ObjectPool<EffectIcon> iconPool;
        private List<EffectIcon> visualizedIcons;
        public void SetUp()
        {
            visualizedIcons = new List<EffectIcon>();
            iconPool = PoolHelper.CreatePool(() => Instantiate(effectIconPrefab, effectIconContainer).GetComponent<EffectIcon>());
        }
        
        public void ClearInspector()
        {
            EnableInspector(false);
        }
        
        public void UpdateEffects(SimpleEntity entity)
        {
            EnableInspector(true);
            RegisterForEntityUpdate(entity);
            
            var statusHandler = entity.StatusHandler;
            var effects = statusHandler.GetActiveEffects().Where(effect => effect.GetData().HasVisual).ToList();
            
            int needed = effects.Count;
            int current = visualizedIcons.Count;

            if (needed == 0)
            {
                EnableInspector(false);
                return;
            }
            
            if (current > needed)
            {
                for (int i = needed; i < current; i++)
                    iconPool.Release(visualizedIcons[i]);

                visualizedIcons.RemoveRange(needed, current - needed);
            }
            
            for (int i = current; i < needed; i++)
            {
                EffectIcon newIcon = iconPool.Get();
                visualizedIcons.Add(newIcon);
            }
            
            AddIcons(effects);
        }
        void AddIcons(List<StatusEffect> effects)
        {
            for (int i = 0; i < effects.Count; i++)
            {
                StatusEffect effect = effects[i];
                visualizedIcons[i].AssignEffect(effect);
            }
        }

        void EnableInspector(bool enable)
        {
            effectIconContainer.gameObject.SetActive(enable);
        }
        void RegisterForEntityUpdate(SimpleEntity entity)
        {
            if(selectedEntity == entity) return;
            
            if(selectedEntity != null) entity.OnEntityUpdate -= UpdateEffects;
            selectedEntity = entity;
            selectedEntity.OnEntityUpdate += UpdateEffects;
        }
    }
}
