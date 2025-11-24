using CursedOnion.Game.Cameras;
using CursedOnion.Locators;
using Reflex.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CursedOnion.Game.Entity.Components
{
    [System.Serializable]
    public class AttackEntityComponent : EntityComponent
    {
        [SerializeField] protected Color attackColor = Color.red;
        
        List<Vector3> reachableTiles = new();
        
        private SimpleEntity target;
        public virtual void VisualizeAttack()
        {
            var grid = AssignedEntity.Grid;
            var position = AssignedEntity.transform.position;
            
            grid.ResetPaint();
            bool isMeleeUnit = AssignedEntity.Stats.SpecialAbilityType is not ArcherAbility;

            grid.TryWorldToGridPosition(position, out Vector3 gridPos);
            if (!isMeleeUnit)
            {
                AStarPathFinder.InsertManhattanAttackGridPositions(reachableTiles, grid, gridPos, 2, false);
            }
            else
            {
                AStarPathFinder.InsertMeleeAttackGridPositions(reachableTiles, grid, gridPos);
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
            if(AssignedEntity is not Unit)
            {
                return false;
            }

            if (!AssignedEntity.Grid.TryWorldToGridPosition(target.transform.position, out Vector3 targetGridPos)) return false;
            return reachableTiles.Contains(targetGridPos);
        }

        public virtual void DoAttack(SimpleEntity target, bool undo)
        {
            this.target = target;
            var camera = AssignedEntity.gameObject.scene.GetSceneContainer().Resolve<RuntimeVariableLocator>().GlobalCamera;

            Transform targetTransform = target.transform;
            Transform attackerTransform = AssignedEntity.transform;
            
            Vector3 direction = (targetTransform.position - attackerTransform.position);
            RotateEntity(camera, AssignedEntity.transform, direction);
            RotateEntity(camera, targetTransform.transform, -direction);

            string anim = AssignedEntity.Stats.SpecialAbilityType is ArcherAbility ? "shoot" : "punch";
            if (AssignedEntity.TryGetLayeredEntity(out var layeredEntity)) layeredEntity.PlayAnimation(anim);
        }

        public void ApplyAttack()
        {
            int finalDamage = CalculateDamage(AssignedEntity, target);
            Debug.Log($"{AssignedEntity.name} ataca a {target.name} causando {finalDamage} de daño.");
            
            target.DamageFrom(finalDamage, AssignedEntity);

            if (AssignedEntity.StatusHandler.HasCounterAttackTarget())
            {
                AssignedEntity.StatusHandler.ResetCounterAttack();
            }
            else
            {
                AssignedEntity.ActionHandler.RaiseFlag(UsedFlags);
            }
        }
        private int CalculateDamage(SimpleEntity attacker, SimpleEntity target)
        {
            int rawDamage = Mathf.CeilToInt(attacker.Stats.AttackStat * attacker.StatusHandler.AttackMultiplier);
            int targetDefense = target.Stats.DefenseStat;
            int finalDamage = Mathf.Max(1, rawDamage - targetDefense);
            
            attacker.StatusHandler.AttackMultiplier = 1;
            return finalDamage;
        }

        private void RotateEntityTowards(GlobalCamera camera, Transform entityTransform, Transform targetTransform)
        {
            Vector3 direction = (targetTransform.position - entityTransform.position);
            direction.y = 0;

            RotateEntity(camera, entityTransform, direction);
        }

        

    }
}