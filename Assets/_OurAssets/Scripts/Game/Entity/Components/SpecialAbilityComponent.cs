using CursedOnion.Locators;
using Reflex.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace CursedOnion.Game.Entity.Components
{
    [System.Serializable]
    public class SpecialAbilityComponent : EntityComponent
    {
        [SerializeField] Color abilityColor = Color.yellow;
        
        private static List<Vector3> reachableTiles = new();
        
        public virtual void VisualizeAbility()
        {
            var stats = AssignedEntity.Stats;
            stats.SpecialAbilityType.InsertReachableTiles(reachableTiles, AssignedEntity);
            AssignedEntity.Grid.PaintTilesAtGridPositions(reachableTiles, abilityColor);
        }
        public virtual void DoAbility(SimpleEntity target, bool undo)
        {
            if (AssignedEntity is not Unit unit || unit.Stats.SpecialAbilityType == null) return;

            unit.Grid.ResetPaint();
            unit.Stats.SpecialAbilityType.ActivateAbility(unit, target);
            AssignedEntity.ActionHandler.RaiseFlag(UsedFlags);

            var camera = AssignedEntity.gameObject.scene.GetSceneContainer().Resolve<RuntimeVariableLocator>().GlobalCamera;

            Transform targetTransform = target.transform;
            Transform attackerTransform = AssignedEntity.transform;

            Vector3 direction = (targetTransform.position - attackerTransform.position);
            RotateEntity(camera, AssignedEntity.transform, direction);
            RotateEntity(camera, targetTransform.transform, -direction);

            string anim = "buff";

            if (AssignedEntity.Stats.SpecialAbilityType is ArcherAbility)
            {
                anim = "shoot";
            }
            else if (AssignedEntity.Stats.SpecialAbilityType is BarbarianAbility)
            {
                anim = "punch";
            }
            else if (AssignedEntity.Stats.SpecialAbilityType is HealerAbility)
            {
                anim = "heal";
            }

            if (AssignedEntity.TryGetLayeredEntity(out var layeredEntity)) layeredEntity.PlayAnimation(anim);

            reachableTiles.Clear();
        }
        public virtual bool ValidateAbility(SimpleEntity target)
        {
            var grid = AssignedEntity.Grid;
            grid.ResetPaint();

            if (target == null)
            {
                Debug.LogWarning("ValidateAbility falló: target es null");
                return false;
            }
            if (!grid.TryWorldToGridPosition(target.transform.position, out Vector3 targetGridPos))
            {
                Debug.LogWarning("ValidateAbility falló: No se pudo convertir target a grid");
                return false;
            }
            if (AssignedEntity.Stats.SpecialAbilityType.SelfTargetOnly) 
                return target == AssignedEntity;
            
            return reachableTiles.Contains(targetGridPos);
        }
    }
}