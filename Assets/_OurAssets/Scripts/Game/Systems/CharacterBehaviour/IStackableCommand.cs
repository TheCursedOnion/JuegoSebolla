namespace CursedOnion
{
    public interface IStackableCommand : ICommand
    {
        void Undo();
        void Redo();
    }
}
