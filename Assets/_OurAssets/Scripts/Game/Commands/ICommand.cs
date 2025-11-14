using System.Windows.Input;
using UnityEngine;

namespace CursedOnion.Game.Commands
{
    public interface ICommand
    {
        bool CanExecute();
        bool Execute();
    }
    
    public interface IStackableCommand : ICommand { void Undo(); }
    public interface IClearStackCommand : ICommand {}

}
