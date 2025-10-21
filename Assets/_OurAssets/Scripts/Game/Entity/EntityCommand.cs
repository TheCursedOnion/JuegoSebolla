using System;
using CursedOnion.Game.Entity;
using UnityEngine;

namespace CursedOnion.Game.Commands
{
    public abstract class EntityCommand : IStackableCommand
    {
        public ICommandEntity Entity;
        protected EntityCommand(ICommandEntity entity)
        {
            this.Entity = entity;
        }

        public static T Create<T>(params object[] args) where T : EntityCommand
        {
            return (T)System.Activator.CreateInstance(typeof(T), args);
        }

        public abstract void Execute();
        public abstract void Undo();
        public abstract void Redo();
    }


}
