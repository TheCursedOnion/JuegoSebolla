using System;
using System.Collections.Generic;
using UnityEngine;

namespace CursedOnion.Game.Entity.Components
{
    [System.Serializable]
    public class AttackEntityComponent : EntityComponent
    {
        [SerializeField] protected Color attackColor = Color.red;
        
        private float nextAttackMultiplier = 1;
        List<Vector3> reachableTiles = new();
        
        public void SetNextAttackMultiplier(float multiplier)
        {
            nextAttackMultiplier = multiplier;
        }

        public virtual void VisualizeAttack()
        {
            var grid = AssignedEntity.Grid;
            var position = AssignedEntity.transform.position;
            
            grid.ResetPaint();
            bool isMeleeUnit = AssignedEntity.GetStats().SpecialAbilityType is not ArcherAbility;

            grid.TryWorldToGridPosition(position, out Vector3 gridPos);
            if (!isMeleeUnit)
            {
                AStarPathFinder.InsertRangeAttackPositions(reachableTiles, grid, gridPos, 2, true);
            }
            else
            {
                AStarPathFinder.InsertMeleeAttackPositions(reachableTiles, grid, gridPos);
            }
            
            grid.PaintTilesAtGridPositions(reachableTiles, attackColor);
        }
        public virtual bool ValidateAttack(SimpleEntity target)
        {
            AssignedEntity.Grid.ResetPaint();
            if (target == null)
            {
                Debug.LogWarning("ValidateAttack falló: target es null");
                return false;
            }

            if (target.GetSide() == AssignedEntity.GetSide())
            {
                Debug.LogWarning($"{AssignedEntity.name} no puede atacar a {target.name} porque son del mismo bando.");
                return false;
            }

            if (!AssignedEntity.Grid.TryWorldToGridPosition(target.transform.position, out Vector3 targetGridPos)) return false;
            return reachableTiles.Contains(targetGridPos);
        }
        public virtual void DoAttack(SimpleEntity target, bool undo)
        {
            int rawDamage = Mathf.CeilToInt(AssignedEntity.GetStats().AttackStat * nextAttackMultiplier);

            int targetDefense = target.GetStats().DefenseStat;
            int finalDamage = Mathf.Max(1, rawDamage - targetDefense);

            Debug.Log($"{AssignedEntity.name} ataca a {target.name} causando {finalDamage} de daño.");

            target.Damage(finalDamage);

            nextAttackMultiplier = 1;
            
            /*if (!target.GetFlags().HasDied() && AssignedEntity.GetStats().SpecialAbilityType is not ArcherAbility)
            {
                int counterDamage = target.GetStats().AttackStat;

                Debug.Log($"{target.name} contraataca a {AssignedEntity.name} causando {counterDamage} de daño.");

                AssignedEntity.Damage(counterDamage);
            }*/
            
            AssignedEntity.GetFlags().RaiseFlag(UsedFlags);
        }
        
    }
}