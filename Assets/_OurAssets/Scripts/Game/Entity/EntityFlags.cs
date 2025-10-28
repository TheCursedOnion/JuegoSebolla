namespace CursedOnion.Game.Entity
{
    public class EntityFlags
    {
        public bool HasDied = false;

        public virtual void ResetFlags()
        {
            
        }
    }
    public class ExtendedEntityFlags : EntityFlags
    {
        public bool HasAttacked = false;
        public bool HasMoved = false;
        
        public override void ResetFlags()
        {
            HasDied = false;
            HasAttacked = false;
            HasMoved = false;
        }
    }
}