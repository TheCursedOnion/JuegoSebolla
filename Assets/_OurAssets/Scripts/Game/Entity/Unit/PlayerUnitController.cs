using UnityEngine;

namespace CursedOnion.Game.Entity
{
    public class PlayerUnitController : UnitController
    {
        public override void ProcessTurn(Unit unit)
        {
            Debug.Log($"Turno de unidad aliada: {unit.name}");
        }
    }
}
