using System;
using System.Collections.Generic;
using CursedOnion.Game.Entity;

namespace CursedOnion.Game.Commands
{
    public class EntityCommand
    {
        protected readonly CommandableEntity CommandSubject;

        public EntityCommand(CommandableEntity commandSubject)
        {
            CommandSubject = commandSubject;
        }
    }
    public abstract class EntityCommandFactory
    {
        private static readonly Dictionary<Type, Func<CommandableEntity, EntityCommandParameters, EntityCommand>> factories =
            new()
            {
                { typeof(MoveCommand), (s, p) => MoveCommand.Create(s, p.Position) },
                { typeof(AttackCommand), (s, p) => AttackCommand.Create(s, p.Target) },
            };
        
        protected readonly CommandableEntity CommandSubject;
        protected EntityCommandFactory(CommandableEntity commandSubject)
        {
            this.CommandSubject = commandSubject;
        }
        
        public static T Create<T>(CommandableEntity commandSubject, EntityCommandParameters parameters) where T : EntityCommand
        {
            if (factories.TryGetValue(typeof(T), out var factory))
                return (T)factory(commandSubject, parameters);

            throw new NotSupportedException($"Unsupported command type: {typeof(T).Name}");
        }

        public abstract bool Execute();
        public abstract void Undo();
        public abstract void Redo();
    }
}
