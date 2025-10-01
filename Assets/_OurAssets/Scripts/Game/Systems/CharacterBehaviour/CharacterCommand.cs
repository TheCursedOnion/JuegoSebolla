using System;
using UnityEngine;
using static UnityEngine.Rendering.GPUSort;

namespace CursedOnion
{
    public abstract class CharacterCommand : IStackableCommand
    {
        public IEntity character;

        protected CharacterCommand(IEntity character)
        {
            this.character = character;
        }

        public static T Create<T>(params object[] args) where T : CharacterCommand
        {
            return (T)System.Activator.CreateInstance(typeof(T), args);
        }

        public abstract void Execute();
        public abstract void Undo();
        public abstract void Redo();
    }


}
