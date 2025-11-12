using UnityEngine;

namespace CursedOnion.Game.Entity
{
    public class PlayerUnitController : EntityComponentController
    {
        public override void ProcessTurn(SimpleEntity unit)
        {
            Debug.Log($"Turno de unidad aliada: {unit.name}");
        }
    }
}
