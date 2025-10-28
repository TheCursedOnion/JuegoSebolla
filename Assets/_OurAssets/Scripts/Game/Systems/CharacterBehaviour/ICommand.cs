using UnityEngine;

namespace CursedOnion.Game.Commands
{
    public interface ICommand { bool Execute(); }
    
    public interface IStackableCommand : ICommand { void Undo(); }
    public interface IClearStackCommand : ICommand {}

}
