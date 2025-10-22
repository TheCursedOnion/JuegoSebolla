using UnityEngine;

namespace CursedOnion.Game.Entity
{
    public abstract class UnitController : MonoBehaviour
    {
        public abstract void ProcessTurn();
    }
    
    public class PlayerUnitController : UnitController
    {
        public override void ProcessTurn()
        {
            throw new System.NotImplementedException();
        }
    }
    public class AIUnitController : UnitController
    {
        public override void ProcessTurn()
        {
            throw new System.NotImplementedException();
        }
    }
}
