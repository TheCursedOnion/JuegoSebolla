using UnityEngine;

namespace CursedOnion.Game.Entity
{
    public class AIUnitController : EntityComponentController
    {
        public override void ProcessTurn(SimpleEntity entity)
        {
            Debug.Log($"Turno de unidad enemiga: {entity.name}");
        }
    }
}
