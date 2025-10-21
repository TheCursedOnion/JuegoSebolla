namespace CursedOnion.Game.Entity
{
    public class EntityFlags
    {
        public bool HasDied = false;
        public bool CanMove = false;
        public bool CanAttack = false;
        public bool HasAttacked = false;
        public bool HasMoved = false;

        public void ResetFlags()
        {
            HasDied = CanMove = CanAttack = HasAttacked = HasMoved = false;
        }
    }
}