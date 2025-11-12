namespace CursedOnion.Game.Entity
{
    public abstract class CommandableEntity : SimpleEntity
    {
        //Stats (They Get Defined)
        protected override EntityStats Stats { get; } = new ExtendedEntityStats();
        public new ExtendedEntityStats GetStats() => Stats as ExtendedEntityStats;
        
        //Flags
        
        
        #region Basic Commands
        public void Attack(SimpleEntity target)
        {
            DoAttack(target, undo: false);
        }

        public void UndoAttack(SimpleEntity target)
        {
            DoAttack(target, undo: true);
        }
        
        protected abstract void DoAttack(SimpleEntity target, bool undo);
        public abstract bool ValidateAttack(SimpleEntity target);

        public void ActivateAbility(SimpleEntity target)
        {
            DoAbility(target, undo: false);
        }

        protected abstract void DoAbility(SimpleEntity target, bool undo);
        public abstract bool ValidateAbility(SimpleEntity target);
        
        #endregion
    }
}
