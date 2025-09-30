using System;
using UnityEngine;

namespace CursedOnion
{
    public abstract class CharacterCommand : IStackableCommand
    {
        public IEntity character;
        protected CharacterCommand(IEntity character)
        {
            this.character = character;
        }
        public abstract void Execute();

        public static T Create<T>(IEntity character) where T : CharacterCommand
        {
            return (T)System.Activator.CreateInstance(typeof(T), character);
        }

        public void Undo() { }
        public void Redo() { }
    }

    public class MoveCommand : CharacterCommand
    {
        public MoveCommand(IEntity character) : base(character) { }
        public override void Execute()
        {
            character.Move();
            var a = CharacterCommand.Create<AttackCommand>(character);
        }
    }
    public class AttackCommand : CharacterCommand
    {
        IEntity target;
        float previousHP;
        public AttackCommand(IEntity character, IEntity target) : base(character) { }
        public override void Execute()
        {
            previousHP = 0;
            character.Attack(target);
        }
    }


}
