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
        
        public void SetNextAttackMultiplier(float multiplier)
        {
            var unit = AssignedEntity as Unit;
            if (unit != null)
            {
                unit.AttackMultiplier = multiplier;
            }
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
            var unit = AssignedEntity as Unit;
            var camera = AssignedEntity.gameObject.scene.GetSceneContainer().Resolve<RuntimeVariableLocator>().GlobalCamera;

            RotateEntityTowards(camera, AssignedEntity.transform, target.transform);
            RotateEntityTowards(camera, target.transform, AssignedEntity.transform);

            if (AssignedEntity.Stats.SpecialAbilityType is not ArcherAbility)
            {
                if (AssignedEntity.TryGetLayeredEntity(out var layeredEntity)) layeredEntity.PlayAnimation("punch");
            }else if (AssignedEntity.Stats.SpecialAbilityType is ArcherAbility)
            {
                if (AssignedEntity.TryGetLayeredEntity(out var layeredEntity)) layeredEntity.PlayAnimation("shoot");
            }
            
            int rawDamage = Mathf.CeilToInt(AssignedEntity.Stats.AttackStat * unit.AttackMultiplier);

            Debug.Log($"{AssignedEntity.name} ataque base: {AssignedEntity.Stats.AttackStat} y multiplicador: {unit.AttackMultiplier}");

            int targetDefense = target.Stats.DefenseStat;
            int finalDamage = Mathf.Max(1, rawDamage - targetDefense);

            Debug.Log($"{AssignedEntity.name} ataca a {target.name} causando {finalDamage} de daño.");

            target.Damage(finalDamage);

            unit.AttackMultiplier = 1;
            
            /*if (!target.GetFlags().HasDied() && AssignedEntity.Stats.SpecialAbilityType is not ArcherAbility)
            {
                int counterDamage = target.Stats.AttackStat;

                Debug.Log($"{target.name} contraataca a {AssignedEntity.name} causando {counterDamage} de daño.");

                var targetCounter = target as Unit;
                target.EntityController.GetComponent<AttackEntityComponent>().DoAttack(AssignedEntity, false);
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