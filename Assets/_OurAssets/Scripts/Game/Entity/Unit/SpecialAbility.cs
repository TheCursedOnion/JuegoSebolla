using CursedOnion.Game.Systems.Grid;
using System;
using System.Collections.Generic;
using CursedOnion.Game.Entity.Components;
using UnityEngine;
using CursedOnion.Game.Systems.Level;

namespace CursedOnion.Game.Entity
{
    [System.Serializable]
    public class SpecialAbility
    {
        public bool SelfTargetOnly = false;
        public int AbilityMinRange = 0;
        public int AbilityMaxRange = 0;

        public virtual void ActivateAbility(Unit unit, SimpleEntity target = null) { }
        public virtual void InsertReachableTiles(List<Vector3> reachablePositionsList, SimpleEntity subject)
        {
            var grid = subject.Grid;
            var transform = subject.transform;
            
            if(grid.TryWorldToGridPosition(transform.position, out Vector3 gridPos))
                AStarPathFinder.InsertMeleeAttackPositions(reachablePositionsList, grid, gridPos);
        }
    }
    
    [System.Serializable]
    public class SoldierAbility : SpecialAbility
    {
        public float DamageMultiplier = 1.3f; 

        public override void ActivateAbility(Unit unit, SimpleEntity target)
        {
            Debug.Log("Activando habilidad de Soldier: Aumentando daño del próximo ataque");
            
            var grid = unit.Grid;
            if (!grid.TryWorldToGridPosition(target.transform.position, out Vector3 targetGridPos)) return;
            if (!grid.TryWorldToGridPosition(unit.transform.position, out Vector3 unitGridPos)) return;

            Vector3 dir = targetGridPos - unitGridPos;
            dir.y = 0;

            if (Mathf.Abs(dir.x) > 0 && Mathf.Abs(dir.z) > 0)
            {
                return;
            }
            
            unit.EntityController.GetEntityComponent<AttackEntityComponent>().SetNextAttackMultiplier(DamageMultiplier);
        }
    }

    [System.Serializable]
    public class TankAbility : SpecialAbility
    {
        public int AdditionalHPFactor = 20;

        public override void ActivateAbility(Unit unit, SimpleEntity target)
        {
            Debug.Log("Activando habilidad de Tank: Aumentando HP adicional");
            unit.SetAdditionalHP(AdditionalHPFactor);
        }
    }

    [System.Serializable]
    public class ThiefAbility : SpecialAbility
    {
        public override void ActivateAbility(Unit unit, SimpleEntity target)
        {
            if (target is Unit targetUnit)
            {
                Debug.Log("Activando habilidad de Thief: Aplicando confusión al objetivo");
                targetUnit.ApplyConfusion(1);
            }
        }

    }

    [System.Serializable]
    public class BarbarianAbility : SpecialAbility
    {
        public override void ActivateAbility(Unit unit, SimpleEntity target)
        {
            if (target is Unit targetUnit)
            {
                Debug.Log("Activando habilidad de Barbarian: Eliminando unidad neutral");
                if (targetUnit.GetSide() == BattleSide.Neutral)
                    targetUnit.Dispose();
            }
        }

    }

    [System.Serializable]
    public class KnightAbility : SpecialAbility
    {
        public override void ActivateAbility(Unit unit, SimpleEntity target)
        {
            Debug.Log("Activando habilidad de Knight: Aumentando movimiento en 2");
            unit.GetStats().MovementStat += 2;
        }

    }

    [System.Serializable]
    public class HealerAbility : SpecialAbility
    {
        public override void ActivateAbility(Unit unit, SimpleEntity target)
        {
            if (target is Unit targetUnit)
            {
                if (targetUnit.GetSide() != unit.GetSide())
                    return;
                Debug.Log("Activando habilidad de Healer: Curando al objetivo");
                int healedAmount = (int)Math.Ceiling(unit.GetStats().CurrentHealthStat * 0.5f);
                targetUnit.Heal(healedAmount);
            }
        }

    }

    [System.Serializable]
    public class ArcherAbility : SpecialAbility
    {
        public override void InsertReachableTiles(List<Vector3> reachablePositionsList, SimpleEntity subject)
        {
            var stats = subject.GetStats();
            var grid = subject.Grid;
            var transform = subject.transform;
            
            var ability = stats.SpecialAbilityType;
            
            if(grid.TryWorldToGridPosition(transform.position, out Vector3 gridPos))
                AStarPathFinder.InsertRangeAttackPositions(reachablePositionsList, grid, gridPos, 2, false);
        }
        
        public override void ActivateAbility(Unit unit, SimpleEntity target)
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

            int damage = Mathf.CeilToInt(unit.GetStats().AttackStat * 0.4f);

            for (int i = 0; i < 3; i++)
            {
                Vector3 posToCheck = targetGridPos + direction * i;

                Tile3d tile = unit.Grid.GetTileAtGridPosition(posToCheck);

                if (tile == null)
                    continue; 

                SimpleEntity entity = tile.GetContainedEntity();

                if (entity == null)
                    continue; 

                if (entity is Unit enemyUnit && enemyUnit.GetSide() != unit.GetSide())
                {
                    enemyUnit.Damage(damage);
                    Debug.Log($"{enemyUnit.name} recibió {damage} puntos de daño por la habilidad de Arquero");
                }
                
            }
        }

    }

}
