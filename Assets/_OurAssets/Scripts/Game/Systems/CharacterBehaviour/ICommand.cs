using UnityEngine;

namespace CursedOnion
{
    public interface ICommand
    {
        void Execute();
    }

    public interface IStackableCommand : ICommand
    {
        void Undo();
        void Redo();
    }
}
