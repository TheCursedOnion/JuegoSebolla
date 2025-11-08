using CursedOnion.Game.Systems.Grid;
using System;
using UnityEngine;
using CursedOnion.Game.Systems.Level;

namespace CursedOnion.Game.Entity
{
    [System.Serializable]
    public class SpecialAbility
    {
        public string Name;
        public string Description;

        public virtual void ActivateAbility(Unit unit, SimpleEntity target = null)
        {
            // Lógica para activar la habilidad especial
        }
        // Otros atributos y métodos relevantes para habilidades especiales
    }
    
    [System.Serializable]
    public class SoldierAbility : SpecialAbility
    {
        public float DamageMultiplier = 2.0f; 

        public override void ActivateAbility(Unit unit, SimpleEntity target)
        {
            if (unit == null) return;
            if (target != unit)
            {
                return;
            }
            unit.SetNextAttackMultiplier(DamageMultiplier);
        }
    }

    [System.Serializable]
    public class TankAbility : SpecialAbility
    {
        public float AdditionalHPFactor = 30.0f;

        public override void ActivateAbility(Unit unit, SimpleEntity target)
        {
            if (unit == null) return;
            if (target != unit)
            {
                return;
            }
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
                if(targetUnit.Side == BattleSide.Neutral)
                    targetUnit.Dispose();
            }
        }

    }

    [System.Serializable]
    public class KnightAbility : SpecialAbility
    {
        public override void ActivateAbility(Unit unit, SimpleEntity target)
        {
            if (target != unit)
            {
                return;
            }
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
