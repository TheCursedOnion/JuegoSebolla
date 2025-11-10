using UnityEngine;

namespace CursedOnion.Game.Entity
{
    public class AIUnitController : UnitController
    {
        public override void ProcessTurn(Unit unit)
        {
            Debug.Log($"Turno de unidad enemiga: {unit.name}");
        }
    }
}
