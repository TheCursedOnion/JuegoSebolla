using UnityEngine;

namespace CursedOnion
{
    public abstract class CharacterCommand : ICommand
    {
        public IEntity character;
        public CharacterCommand(IEntity character)
        {
            this.character = character;
        }
        public abstract void Execute();

        public static T Create<T>(IEntity character) where T : CharacterCommand
        {
            return (T) System.Activator.CreateInstance(typeof(T), character);
        }
    }
    public class MoveCommand : CharacterCommand
    {
        public MoveCommand(IEntity character) : base(character) { }
        public override void Execute()
        {
            character.Move();
        }
    }
    public class AttackCommand : CharacterCommand
    {
        public AttackCommand(IEntity character) : base(character) { }
        public override void Execute()
        {
            character.Attack();
        }
    }
}
