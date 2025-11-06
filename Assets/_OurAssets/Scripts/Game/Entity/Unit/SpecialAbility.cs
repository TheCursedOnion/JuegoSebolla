namespace CursedOnion.Game.Entity
{
    [System.Serializable]
    public class SpecialAbility
    {
        public string Name;
        public string Description;

        public virtual void ActivateAbility(Unit unit, SimpleEntity target = null)
        {
            // Lógica para activar la habilidad especial
        }
        // Otros atributos y métodos relevantes para habilidades especiales
    }
    
    [System.Serializable]
    public class SoldierAbility : SpecialAbility
    {
        public float DamageMultiplier = 2.0f; 

        public override void ActivateAbility(Unit unit, SimpleEntity target)
        {
            if (unit == null) return;
            if (target != unit)
            {
                return;
            }
            unit.SetNextAttackMultiplier(DamageMultiplier);
        }
    }

    [System.Serializable]
    public class TankAbility : SpecialAbility
    {
        public float AdditionalHPFactor = 30.0f;

        public override void ActivateAbility(Unit unit, SimpleEntity target)
        {
            if (unit == null) return;
            if (target != unit)
            {
                return;
            }
            unit.SetAdditionalHP(AdditionalHPFactor);
        }
    }

    [System.Serializable]
    public class ThiefAbility : SpecialAbility
    {
        public override void ActivateAbility(Unit unit, SimpleEntity target)
        {
            if (target is Unit targetUnit)
            {
                targetUnit.ApplyConfusion(1);
            }
        }

    }
}
