using CursedOnion.Game.Modes.General.Animations;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.Game.Systems.Level;
using NaughtyAttributes;
using Reflex.Attributes;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace CursedOnion.Game.Entity
{
    public class SimpleEntity : MonoBehaviour
    {
        [HideInInspector, Inject] public LevelManager LevelManager;
        [HideInInspector] public LevelEvents LevelEvents;
        [HideInInspector] public Grid3d Grid;

        [HorizontalLine(height: 2f, color: EColor.Violet)]
        [SerializeField] protected BattleSide EntitySide = BattleSide.Neutral;
        public bool IsBreakable = false;

        public EntityComponentController EntityController;
        [SerializeField] protected LayeredEntity LayeredEntity;
        public LayeredEntity GetLayeredEntity() => LayeredEntity;
        
        [HorizontalLine(height: 2f, color: EColor.Violet)]
        [FormerlySerializedAs("Data")]
        [Expandable] public StatData StatData;
        public ExtendedEntityStats Stats;
        
        public bool HasTurn = false;
        public ActionHandler ActionHandler;
        public StatusHandler StatusHandler;
        
        public event Action<SimpleEntity> OnEntityUpdate;
        public void NotifyActionUpdate() => OnEntityUpdate?.Invoke(this);
        
        public event Action OnStartTurn;
        public void NotifyStartTurn() => OnStartTurn?.Invoke();
        
        protected virtual void Awake()
        {
            Stats = new ExtendedEntityStats();
        }
        protected virtual void Start()
        {
            DefineEntityStats(StatData);
            SetLevelVariables(LevelManager);
            SetComponents();
        }

        protected virtual void SetComponents()
        {
            switch (GetSide())
            {
                case BattleSide.Ally:
                    EntityController ??= gameObject.GetOrAddComponent<PlayerUnitController>();
                    break;
                case BattleSide.Enemy:
                    EntityController ??= gameObject.GetOrAddComponent<AIUnitController>();
                    break;
                case BattleSide.Neutral:
                    EntityController ??= gameObject.GetOrAddComponent<EntityComponentController>();
                    break;
            }
            EntityController.Initialize(this, StatData.EntityComponents);
            EntityController.PlaceComponent.PlaceEntity();
        }

        public void Dispose()
        {
            EntityController.Dispose();
            Destroy(gameObject);
        }
        protected virtual void DefineEntityStats(StatData data)
        {
            StatData = data;
            Stats.SetWithData(data);
            
            ActionHandler = new ActionHandler(this);
            StatusHandler = new StatusHandler(this, Stats);
        }
        public BattleSide GetSide() => EntitySide;
        public bool TryGetLayeredEntity(out LayeredEntity layeredEntity)
        {
            layeredEntity = this.LayeredEntity;
            return layeredEntity != null;
        }
        protected virtual void SetLevelVariables(LevelManager manager)
        {
            LevelManager = manager;
            LevelEvents = LevelManager.LevelEvents;
            Grid = LevelManager.Grid;
        }
        
        public virtual void DamageFrom(int damage, SimpleEntity attacker)
        {
            int remainingDamage = StatusHandler.GetRemainingDamage(damage);
            Stats.CurrentHealthStat -= remainingDamage;
            
            if (Stats.CurrentHealthStat <= 0)
            {
                Die();
            }
        }
        public virtual void Heal(int healedHP)
        {
            Stats.CurrentHealthStat = Math.Min(Stats.CurrentHealthStat + healedHP, Stats.MaxHealthStat);
        }
        protected void Die()
        {
            ActionHandler.RaiseFlag(ActionFlag.HasDied);
            OnEntityUpdate?.Invoke(this);
            Dispose();
        }
        
        //No se llama nunca pero lo dejo por si acaso
        public virtual void Revive(int newHealth)
        {
            Stats.CurrentHealthStat = newHealth;
            ActionHandler.ResetFlag(ActionFlag.HasDied);
            
            OnEntityUpdate?.Invoke(this);
        }

        public void UpdateStatusEffects()
        {
            StatusHandler.UpdateStatusEffects();
        }
    }
}