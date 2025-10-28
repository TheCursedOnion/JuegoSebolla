using CursedOnion.Game.Logic;

namespace CursedOnion.Game.Events
{
    public class RuntimeEvents
    {
        protected bool Enabled;
        public RuntimeEvents()
        {
            EnableEvents(true);   
        }
        
        public void EnableEvents(bool enable) => Enabled = enable;
    }
}