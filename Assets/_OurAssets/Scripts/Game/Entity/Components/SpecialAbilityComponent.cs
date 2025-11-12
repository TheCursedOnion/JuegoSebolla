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
            var stats = AssignedEntity.GetStats();
            stats.SpecialAbilityType.InsertReachableTiles(reachableTiles, AssignedEntity);
            AssignedEntity.Grid.PaintTilesAtGridPositions(reachableTiles, abilityColor);
        }
        public virtual void DoAbility(SimpleEntity target, bool undo)
        {
            if(AssignedEntity is not Unit unit) return;
            
            unit.Grid.ResetPaint();
            unit.GetStats().SpecialAbilityType.ActivateAbility(unit, target);
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
            if (AssignedEntity.GetStats().SpecialAbilityType.SelfTargetOnly) 
                return target != AssignedEntity;
            
            return reachableTiles != null && reachableTiles.Contains(target.transform.position);
        }
    }
}