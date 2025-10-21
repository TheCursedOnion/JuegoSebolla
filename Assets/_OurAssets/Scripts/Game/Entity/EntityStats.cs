namespace CursedOnion.Game.Entity
{
    public class EntityStats
    {
        public int CurrentHealthStat;
        public int MaxHealthStat;
        
        public int AttackStat;
        public int DefenseStat;
        public int InitiativeStat;
        public int MovementStat;
        public int PriceStat;
        public void SetStats(EntityData data)
        {
            CurrentHealthStat = MaxHealthStat = data.GetRandomHP();
            AttackStat = data.GetRandomAttack();
            DefenseStat = data.GetRandomDefense();
            InitiativeStat = data.GetRandomInitiative();
            MovementStat = data.GetMovement();
            PriceStat = data.GetPrice();
        }
    }
}