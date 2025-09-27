namespace CursedOnion.Game.Systems.Grid
{
    [System.Serializable]
    public class Tile3dDescriptor
    {
        public static Tile3dDescriptor Default
        {
            get
            {
                var defaultDescriptor = new Tile3dDescriptor
                {
                    Id = 0,
                    Cost = 0
                };
                return defaultDescriptor;
            }
        }
        public uint Id;
        public int Cost;
    }
}