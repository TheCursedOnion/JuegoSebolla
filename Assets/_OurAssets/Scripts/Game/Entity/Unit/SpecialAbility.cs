namespace CursedOnion.Game.Entity
{
    [System.Serializable]
    public class SpecialAbility
    {
        public string Name;
        public string Description;

        public void ActivateAbility(Unit unit)
        {
            // Lógica para activar la habilidad especial
        }
        // Otros atributos y métodos relevantes para habilidades especiales
    }
    
    [System.Serializable]
    public class ArcherAbility : SpecialAbility
    {
        public float RangeMultiplier = 1.5f;
    }
}
