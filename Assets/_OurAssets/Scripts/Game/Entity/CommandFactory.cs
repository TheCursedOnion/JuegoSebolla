using System;
using System.Collections.Generic;
using CursedOnion.Game.Entity;

namespace CursedOnion.Game.Commands
{
    public abstract class CommandFactory
    {
        private static readonly Dictionary<Type, Action<CommandParameters>> preFactories =
            new()
            {
                { typeof(MoveCommand), MoveCommand.Prepare },
                { typeof(AttackCommand), AttackCommand.Prepare },
                { typeof(AbilityCommand), AbilityCommand.Prepare },
            };
        
        private static readonly Dictionary<Type, Func<CommandParameters, ICommand>> factories =
            new()
            {
                { typeof(MoveCommand), MoveCommand.Create },
                { typeof(AttackCommand), AttackCommand.Create },
                { typeof(AbilityCommand), AbilityCommand.Create },
                { typeof(SpawnCommand), SpawnCommand.Create },
                { typeof(EraseCommand), EraseCommand.Create},
                { typeof(ActionCommand), ActionCommand.Create}
            };
        
        public static void PreVisualize<T>(CommandParameters parameters) where T : ICommand
        {
            if (preFactories.TryGetValue(typeof(T), out var preVisualize))
                preVisualize(parameters);
        }
        public static T Create<T>(CommandParameters parameters) where T : ICommand
        {
            if (factories.TryGetValue(typeof(T), out var factory))
                return (T)factory(parameters);

            throw new NotSupportedException($"Unsupported command type: {typeof(T).Name}");
        }
    }
}
