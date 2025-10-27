using CursedOnion.Game.Logic;

namespace CursedOnion.Game.Events
{
    interface IRuntimeEvents
    {
        public void EnableEvents(bool enable);
    }
    public class RuntimeEvents : IRuntimeEvents
    {
        protected bool Enabled;
        public RuntimeEvents()
        {
            EnableEvents(true);   
        }
        
        public void EnableEvents(bool enable) => Enabled = enable;
    }
}