using System;
using CursedOnion.Game.Events;
using CursedOnion.Game.Modes.General.Animations;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.Game.Systems.Level;
using NaughtyAttributes;
using Reflex.Extensions;
using UnityEngine;

namespace CursedOnion.Game.Entity
{
    public class SimpleEntity : MonoBehaviour
    {
        [HideInInspector] public LevelManager LevelManager;
        [HideInInspector] public LevelEvents LevelEvents;
        [HideInInspector] public Grid3d Grid;
        
        [HorizontalLine(height: 2f, color: EColor.Violet)]
        public EntityComponentController EntityController;
        [SerializeField] protected LayeredEntity layeredEntity;
        public Action OnEntityUpdate;
        
        [HorizontalLine(height: 2f, color: EColor.Violet)]
        [Expandable] public EntityData Data;
        
        public bool TryGetLayeredEntity(out LayeredEntity layeredEntity)
        {
            layeredEntity = this.layeredEntity;
            return layeredEntity != null;
        }
        protected void SetLevelVariables()
        {
            var container = this.gameObject.scene.GetSceneContainer();
            
            LevelManager = container.Resolve<LevelManager>();
            LevelEvents = LevelManager.LevelEvents;
            Grid = LevelManager.LevelAsset.Grid;
        }
        
        //Stats (They Get Defined)
        protected virtual EntityStats Stats { get; } = new EntityStats();
        public EntityStats GetStats() => Stats;
        
        //Flags
        protected virtual EntityFlags Flags { get; } = new EntityFlags();
        public EntityFlags GetFlags() => Flags;

        public virtual void Damage(int damage)
        {
            Stats.CurrentHealthStat -= damage;
            if (Stats.CurrentHealthStat <= 0) Die();
        }

        public virtual void Heal(int healedHP)
        {
            Stats.CurrentHealthStat = Math.Min(Stats.CurrentHealthStat + healedHP, Stats.MaxHealthStat);
        }
        public virtual void Die()
        {
            GetFlags().HasDied = true;
            OnEntityUpdate?.Invoke();
            Dispose();
        }

        public virtual void Revive(int newHealth)
        {
            Stats.CurrentHealthStat = newHealth;
            GetFlags().HasDied = false;
            
            OnEntityUpdate?.Invoke();
        }
        
        public void Dispose()
        {
            Destroy(gameObject);
        }
    }
}