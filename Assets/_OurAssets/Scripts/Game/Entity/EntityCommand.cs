using System;
using System.Collections.Generic;
using CursedOnion.Game.Entity;
using UnityEngine;

namespace CursedOnion.Game.Commands
{
    public class EntityCommandParameters
    {
        public Vector3 Position;
        public SimpleEntity Target;

        public EntityCommandParameters(Vector3 position, SimpleEntity target)
        {
            Position = position;
            Target = target;
        }
    }
    public abstract class EntityCommand : IStackableCommand
    {
        private static readonly Dictionary<Type, Func<CommandableEntity, EntityCommandParameters, EntityCommand>> factories =
            new()
            {
                { typeof(MoveCommand), (s, p) => MoveCommand.Create(s, p.Position) },
                { typeof(AttackCommand), (s, p) => AttackCommand.Create(s, p.Target) },
            };
        
        protected readonly CommandableEntity CommandSubject;
        protected EntityCommand(CommandableEntity commandSubject)
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
