namespace CursedOnion.Game.Inputs
{
    public interface IInputReader
    {
        public PlayerInputActions InputActions { get; set; }
        public void Enable();
        public void Disable();
    }
}