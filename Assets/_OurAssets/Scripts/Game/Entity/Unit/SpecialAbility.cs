using CursedOnion.Game.Systems.Grid;
using System;
using UnityEngine;
using CursedOnion.Game.Systems.Level;

namespace CursedOnion.Game.Entity
{
    [System.Serializable]
    public class SpecialAbility
    {
        public bool SelfTargetOnly = false;
        public int AbilityRange = 0;

        public virtual void ActivateAbility(Unit unit, SimpleEntity target = null) { }
    
    }
    
    [System.Serializable]
    public class SoldierAbility : SpecialAbility
    {
        public int DamageMultiplier = 2; 

        public override void ActivateAbility(Unit unit, SimpleEntity target)
        {
            Debug.Log("Activando habilidad de Soldier: Aumentando daño del próximo ataque");
            unit.SetNextAttackMultiplier(DamageMultiplier);
        }
    }

    [System.Serializable]
    public class TankAbility : SpecialAbility
    {
        public int AdditionalHPFactor = 30;

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
                if (targetUnit.Side == BattleSide.Neutral)
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
                if (targetUnit.Side != unit.Side)
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
        public override void ActivateAbility(Unit unit, SimpleEntity target)
        {
            if (target is not Unit targetUnit)
                return;

            Vector3 unitPos = unit.transform.position;
            Vector3 targetPos = target.transform.position;

            Vector2 direction2D = new Vector2(
                targetPos.x - unitPos.x,
                targetPos.z - unitPos.z
            );

            Vector3 direction3D = new Vector3(
                direction2D.x,
                0,
                direction2D.y
            );

            int damage = (int)Math.Ceiling(unit.GetStats().AttackStat * 0.5f);

            for (int i = 0; i < 3; i++)
            {
                Vector3 posToCheck = targetPos + direction3D * i;

                Tile3d tile = unit.GetGrid().GetTileAtGridPosition(posToCheck);

                if (tile == null)
                    continue; 

                SimpleEntity entity = tile.GetContainedEntity();

                if (entity == null)
                    continue; 

                if (entity is Unit enemyUnit && enemyUnit.Side != unit.Side)
                {
                    enemyUnit.Damage(damage);
                }
            }
        }

    }

}
