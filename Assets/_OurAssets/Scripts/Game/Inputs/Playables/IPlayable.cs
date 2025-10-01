namespace CursedOnion.Game.Inputs
{
    public interface IPlayable
    {
        public InputReader InputReader { get; set; }
        public string UsedMap {get; set;}
        public void OnEnable();
        public void OnDisable();
    }
}
