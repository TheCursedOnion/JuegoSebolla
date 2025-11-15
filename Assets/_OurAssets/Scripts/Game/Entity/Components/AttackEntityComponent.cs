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
            bool isMeleeUnit = AssignedEntity.Stats.SpecialAbilityType is not ArcherAbility;

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
            var camera = AssignedEntity.gameObject.scene.GetSceneContainer().Resolve<CameraLocator>().GlobalCamera;

            RotateEntityTowards(camera, AssignedEntity.transform, target.transform);
            RotateEntityTowards(camera, target.transform, AssignedEntity.transform);

            if (AssignedEntity.Stats.SpecialAbilityType is not ArcherAbility)
            {
                if (AssignedEntity.TryGetLayeredEntity(out var layeredEntity)) layeredEntity.PlayAnimation("punch");
            }else if (AssignedEntity.Stats.SpecialAbilityType is ArcherAbility)
            {
                if (AssignedEntity.TryGetLayeredEntity(out var layeredEntity)) layeredEntity.PlayAnimation("shoot");
            }

            int rawDamage = Mathf.CeilToInt(AssignedEntity.Stats.AttackStat * nextAttackMultiplier);

            int targetDefense = target.Stats.DefenseStat;
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

        private void RotateEntityTowards(GlobalCamera camera, Transform entityTransform, Transform targetTransform)
        {
            Vector3 direction = (targetTransform.position - entityTransform.position);
            direction.y = 0;

            RotateEntity(camera, entityTransform, direction);
        }

        private void RotateEntity(GlobalCamera camera, Transform transform, Vector3 movementDirection)
        {
            float degrees = camera.GetCameraPanAngles();
            movementDirection = Quaternion.AngleAxis(-degrees, Vector3.up) * movementDirection;

            if (movementDirection.x > 0.01f)
            {
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x) * -1f;
                transform.localScale = scale;
            }
            else if (movementDirection.x < -0.01f)
            {
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x);
                transform.localScale = scale;
            }
        }

    }
}