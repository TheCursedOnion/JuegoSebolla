using UnityEngine;

namespace CursedOnion.Game.Entity
{
    public class AIUnitController : EntityComponentController
    {
        public override void ProcessTurn()
        {
            
        }

        public bool isTrue()
        {
            return true;
        }

        public void HaEntrado()
        {
            Debug.Log("HA ENTRADO");
        }

        public void Salchipapa()
        {
            Debug.Log("Salchipapa!");
        }
    }
}
