using System;
using System.Collections.Generic;
using CursedOnion.Game.Entity;

namespace CursedOnion.Game.Commands
{
    
    //TODO: Quitarla
    public class EntityCommand
    {
        protected readonly SimpleEntity CommandSubject;

        public EntityCommand(SimpleEntity commandSubject)
        {
            CommandSubject = commandSubject;
        }
    }
    public abstract class CommandFactory
    {
        private static readonly Dictionary<Type, Action<SimpleEntity>> preFactories =
            new()
            {
                { typeof(MoveCommand), MoveCommand.Prepare },
                { typeof(AttackCommand), AttackCommand.Prepare },
                { typeof(AbilityCommand), AbilityCommand.Prepare },
            };
        
        private static readonly Dictionary<Type, Func<CommandParameters, ICommand>> factories =
            new()
            {
                { typeof(MoveCommand), (p) => MoveCommand.Create(p.Subject, p.Position.Value) },
                { typeof(AttackCommand), (p) => AttackCommand.Create(p.Subject, p.Target) },
                { typeof(AbilityCommand), (p) => AbilityCommand.Create(p.Subject, p.Target) },
                { typeof(SpawnCommand), (p) =>
                    {
                        if(p.Target) return null;
                        return SpawnCommand.Create(p.EntityPrefab, p.Position.Value, p.TargetTile);
                    }
                },
                { typeof(EraseCommand), (p) => EraseCommand.Create(p.LevelManager, p.TargetTile)},
                { typeof(ActionCommand), (p) => ActionCommand.Create(p.ExecuteAction)}
            };
        
        public static void PreVisualize<T>(SimpleEntity parameters) where T : ICommand
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
