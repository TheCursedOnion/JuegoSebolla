using CursedOnion.Game.Systems.Grid;
using System;
using System.Collections.Generic;
using CursedOnion.Game.Entity.Effects;
using NaughtyAttributes;
using UnityEngine;

namespace CursedOnion.Game.Entity
{
    [Flags]
    public enum StatFlag
    {
        None = 0,
        
        Health = 1 << 0,
        Damage = 1 << 1,
        Defense = 1 << 2,
        Initiative = 1 << 3,
        Movement = 1 << 4,
        Price = 1 << 5,
    }
    
    [System.Serializable]
    public class SpecialAbility
    {
        public Sprite AbilityIcon;
        public bool SelfTargetOnly = false;
        [SerializeField] protected StatFlag AffectedStats;
        public virtual void ActivateAbility(Unit unit, SimpleEntity target = null) { }
        public virtual void InsertReachableTiles(List<Vector3> reachablePositionsList, SimpleEntity subject)
        {
            var grid = subject.Grid;
            var transform = subject.transform;
            
            if(grid.TryWorldToGridPosition(transform.position, out Vector3 gridPos))
                AStarPathFinder.InsertManhattanAttackGridPositions(reachablePositionsList, grid, gridPos, 1, false);
        }
        
        public StatFlag GetAffectedStats() => AffectedStats;
        public override string ToString() => string.Empty;
    }
    
    [System.Serializable]
    public class SoldierAbility : SpecialAbility
    {
        [SerializeField] private float damageMultiplier = 1.3f;

        public override void InsertReachableTiles(List<Vector3> reachablePositionsList, SimpleEntity subject)
        {
            var grid = subject.Grid;

            if (grid.TryWorldToGridPosition(subject.transform.position, out Vector3 gridPos))
            {
                reachablePositionsList.Clear();

                reachablePositionsList.Add(gridPos);
            }
        }

        public override void ActivateAbility(Unit unit, SimpleEntity _ = null)
        {
            Debug.Log("Activando habilidad de Soldier: Aumentando daño del próximo ataque");
            var attackBoost = EntityEffectFactory.CreateEffect<AttackBoostEffect>(-1, damageMultiplier);
            unit.StatusHandler.AddEffect(attackBoost);
        }

        public override string ToString() => " x " + damageMultiplier;
    }

    [System.Serializable]
    public class TankAbility : SpecialAbility
    {
        [SerializeField] private float additionalHPFactor = 0.2f;

        public override void InsertReachableTiles(List<Vector3> reachablePositionsList, SimpleEntity subject)
        {
            var grid = subject.Grid;

            if (grid.TryWorldToGridPosition(subject.transform.position, out Vector3 gridPos))
            {
                reachablePositionsList.Clear();

                reachablePositionsList.Add(gridPos);
            }
        }
        public override void ActivateAbility(Unit unit, SimpleEntity target = null)
        {
            Debug.Log("Activando habilidad de Tank: Aumentando HP adicional");
            var healthBoost = EntityEffectFactory.CreateEffect<HealthBoostEffect>(-1, additionalHPFactor);
            unit.StatusHandler.AddEffect(healthBoost);
        }
        
        public override string ToString() => " + " + additionalHPFactor;
    }

    [System.Serializable]
    public class ThiefAbility : SpecialAbility
    {
        [SerializeField] private int confusionDuration = -1;
        public override void InsertReachableTiles(List<Vector3> reachablePositionsList, SimpleEntity subject)
        {
            var grid = subject.Grid;
            var transform = subject.transform;

            if (grid.TryWorldToGridPosition(transform.position, out Vector3 gridPos))
                AStarPathFinder.InsertManhattanAttackGridPositions(reachablePositionsList, grid, gridPos, 1, false);
        }

        public override void ActivateAbility(Unit unit, SimpleEntity target = null)
        {
            if (target is Unit targetUnit)
            {
                Debug.Log("Activando habilidad de Thief: Aplicando confusión al objetivo");
                int finalDuration = targetUnit.GetSide() == BattleSide.Ally? confusionDuration : -1;
                var confusion = EntityEffectFactory.CreateEffect<ConfusionEffect>(finalDuration, -1);
                targetUnit.StatusHandler.AddEffect(confusion);
            }
        }

    }

    [System.Serializable]
    public class BarbarianAbility : SpecialAbility
    {
        public override void InsertReachableTiles(List<Vector3> reachablePositionsList, SimpleEntity subject)
        {
            var grid = subject.Grid;
            var transform = subject.transform;

            if (grid.TryWorldToGridPosition(transform.position, out Vector3 gridPos))
                AStarPathFinder.InsertManhattanAttackGridPositions(reachablePositionsList, grid, gridPos, 1, false);
        }

        public override void ActivateAbility(Unit unit, SimpleEntity target = null)
        {
            if (target != null)
            {
                Debug.Log("Activando habilidad de Barbarian: Eliminando unidad neutral");
                if (target.IsBreakable)
                    target.Dispose();
            }
        }

    }

    [System.Serializable]
    public class ExplorerAbility : SpecialAbility
    {
        [SerializeField] private int movementBonus = 2;
        public override void InsertReachableTiles(List<Vector3> reachablePositionsList, SimpleEntity subject)
        {
            var grid = subject.Grid;

            if (grid.TryWorldToGridPosition(subject.transform.position, out Vector3 gridPos))
            {
                reachablePositionsList.Clear();

                reachablePositionsList.Add(gridPos);
            }
        }
        public override void ActivateAbility(Unit unit, SimpleEntity target = null)
        {
            Debug.Log("Activando habilidad de Explorer: Aumentando movimiento en 2");
            var moveBoost = EntityEffectFactory.CreateEffect<MovementBoostEffect>(-1, movementBonus);
            unit.StatusHandler.AddEffect(moveBoost);
        }
        public override string ToString() => " + " + movementBonus;

    }

    [System.Serializable]
    public class HealerAbility : SpecialAbility
    {
        public override void InsertReachableTiles(List<Vector3> reachablePositionsList, SimpleEntity subject)
        {
            var grid = subject.Grid;
            var transform = subject.transform;

            if (grid.TryWorldToGridPosition(transform.position, out Vector3 gridPos))
                AStarPathFinder.InsertManhattanAttackGridPositions(reachablePositionsList, grid, gridPos, 1, false);
        }

        public override void ActivateAbility(Unit unit, SimpleEntity target = null)
        {
            if (target is Unit targetUnit)
            {
                if (targetUnit.GetSide() != unit.GetSide()) return;
                Debug.Log("Activando habilidad de Healer: Curando al objetivo");
                int healedAmount = (int)Math.Ceiling(unit.Stats.MaxHealthStat * 0.5f);
                targetUnit.Heal(healedAmount);
            }
        }

    }

    [System.Serializable]
    public class ArcherAbility : SpecialAbility
    {
        public override void InsertReachableTiles(List<Vector3> reachablePositionsList, SimpleEntity subject)
        {
            var stats = subject.Stats;
            var grid = subject.Grid;
            var transform = subject.transform;
            
            var ability = stats.SpecialAbilityType;
            
            if(grid.TryWorldToGridPosition(transform.position, out Vector3 gridPos))
                AStarPathFinder.InsertRangedAttackGridPositions(reachablePositionsList, grid, gridPos, 2);
        }
        
        public override void ActivateAbility(Unit unit, SimpleEntity target = null)
        {

            var grid = unit.Grid;
            if (!grid.TryWorldToGridPosition(unit.transform.position, out Vector3 unitGridPos) ||
                !grid.TryWorldToGridPosition(target.transform.position, out Vector3 targetGridPos))
                return;

            Vector3 direction = targetGridPos - unitGridPos;
            direction.y = 0;

            if (Mathf.Abs(direction.x) > 0 && Mathf.Abs(direction.z) > 0)
            {
                Debug.Log("Arquero solo puede usar habilidad en líneas cardinales");
                return;
            }

            direction = new Vector3(
                Mathf.Clamp(direction.x, -1, 1),
                0,
                Mathf.Clamp(direction.z, -1, 1)
            );

            int damage = Mathf.CeilToInt(unit.Stats.AttackStat * 0.4f);

            for (int i = 0; i < 3; i++)
            {
                Vector3 posToCheck = targetGridPos + direction * i;

                Tile3d tile = unit.Grid.GetTileAtGridPosition(posToCheck);

                if (tile == null)
                    continue; 

                SimpleEntity nextTarget = tile.GetContainedEntity();

                if (nextTarget == null) continue; 

                if (nextTarget is Unit enemyUnit && enemyUnit.GetSide() != unit.GetSide())
                {
                    enemyUnit.DamageFrom(damage, unit);
                    Debug.Log($"{enemyUnit.name} recibió {damage} puntos de daño por la habilidad de Arquero");
                }
                
            }
        }
    }

    [System.Serializable]
    public class PersianBossAbility : SpecialAbility
    {
        [SerializeField] private float damageMultiplier = 1.2f;

        public override void ActivateAbility(Unit unit, SimpleEntity target = null)
        {
            Debug.Log("Activando habilidad de BossPersa: Aumentando daño del próximo ataque de unidad cercanas");
            var attackBoost = EntityEffectFactory.CreateEffect<AttackBoostEffect>(-1, damageMultiplier);
            target.StatusHandler.AddEffect(attackBoost);
        }

        public override string ToString() => " x " + damageMultiplier;
    }

    [System.Serializable]
    public class RobSpecialAbility : SpecialAbility
    {
        [SerializeField] private int explosionDamage = 8;
        public override void ActivateAbility(Unit unit, SimpleEntity target = null)
        { 
            if (target == null) return;
            target.DamageFrom(explosionDamage, unit);
            Debug.Log($"{target.name} recibió {explosionDamage} puntos de daño por la habilidad de Rob (Ha explotado el loco)");
        }

    }

    [System.Serializable]
    public class JeanneSpecialAbility : SpecialAbility
    {
        public override void ActivateAbility(Unit unit, SimpleEntity target = null)
        {
            Debug.Log($"Jeanne haciendo su habilidad...");
        }

    }

}
