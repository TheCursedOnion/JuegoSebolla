using CursedOnion.Game.Logic;
using CursedOnion.Game.Logic.Services;
using UnityEngine;

namespace CursedOnion.Game.Events
{
    public class RuntimeEvents : MonoBehaviour
    {
        protected bool Enabled;
        public RuntimeEvents()
        {
            EnableEvents(true);   
        }
        
        public void EnableEvents(bool enable) => Enabled = enable;
    }
}