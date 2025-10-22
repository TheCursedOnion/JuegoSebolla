using System;
using CursedOnion.Game.Entity;
using UnityEngine;

namespace CursedOnion.Game.Commands
{
    public abstract class EntityCommand : IStackableCommand
    {
        protected readonly CommandableEntity CommandSubject;
        protected EntityCommand(CommandableEntity commandSubject)
        {
            this.CommandSubject = commandSubject;
        }

        public abstract void Execute();
        public abstract void Undo();
        public abstract void Redo();
    }


}
