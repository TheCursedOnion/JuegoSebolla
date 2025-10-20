namespace CursedOnion.Game.Inputs
{
    public interface IController
    {
        public InputReaderCollection InputReaderCollection { get; set; }
        public void Enable();
        public void Disable();
    }
}
