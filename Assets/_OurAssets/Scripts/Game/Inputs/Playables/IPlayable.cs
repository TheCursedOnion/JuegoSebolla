namespace CursedOnion.Game.Inputs
{
    public interface IPlayable
    {
        public InputReaderCollection InputReaderCollection { get; set; }
        public void OnEnable();
        public void OnDisable();
    }
}
