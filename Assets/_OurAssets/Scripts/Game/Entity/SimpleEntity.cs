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
        [SerializeField] protected BattleSide EntitySide = BattleSide.Neutral;
        
        [ReadOnly] public EntityComponentController EntityController;
        [SerializeField] protected LayeredEntity layeredEntity;
        
        [HorizontalLine(height: 2f, color: EColor.Violet)]
        [Expandable] public EntityData Data;
        
        //Stats (They Get Defined)
        protected ExtendedEntityStats Stats;
        public ExtendedEntityStats GetStats() => Stats;
        
        //Flags
        protected EntityFlags Flags;
        public EntityFlags GetFlags() => Flags;
        public event Action<SimpleEntity> OnEntityUpdate;
        public void NotifyUpdate() => OnEntityUpdate?.Invoke(this);
        protected void Awake()
        {
            Flags = new EntityFlags(this);
            Stats = new ExtendedEntityStats();
        }
        
        public BattleSide GetSide() => EntitySide;
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
            GetFlags().RaiseFlag(EntityFlag.HasDied);
            OnEntityUpdate?.Invoke(this);
            Dispose();
        }

        public virtual void Revive(int newHealth)
        {
            Stats.CurrentHealthStat = newHealth;
            GetFlags().ResetFlag(EntityFlag.HasDied);
            
            OnEntityUpdate?.Invoke(this);
        }
        
        public void Dispose()
        {
            Destroy(gameObject);
        }
        
        void OnDisable()
        {
            EntityController.PlaceEntityComponent.Remove();
        }
    }
}