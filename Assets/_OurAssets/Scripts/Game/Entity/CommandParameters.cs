using CursedOnion.Game.Entity;
using UnityEngine;

namespace CursedOnion.Game.Commands
{
    public class CommandParameters
    {
        public CommandableEntity Subject;
        
        public Vector3? Position;
        public SimpleEntity Target;
        public GameObject EntityPrefab;
        
        public void Combine(CommandParameters other)
        {
            if(other == null) return;
            
            Subject ??= other.Subject;
            Target ??= other.Target;
            EntityPrefab ??= other.EntityPrefab;
            Position ??= other.Position;
        }
        
        private CommandParameters()
        {
            
        }
        public class Builder
        {
            private CommandParameters parameters = new CommandParameters();

            public Builder SetCommandSubject(CommandableEntity commandSubject)
            {
                parameters.Subject = commandSubject;
                return this;
            }
            public Builder SetPosition(Vector3 position)
            {
                parameters.Position = position;
                return this;
            }
            public Builder SetTarget(SimpleEntity target)
            {
                parameters.Target = target;
                return this;
            }
            public Builder SetEntityPrefab(GameObject entityPrefab)
            {
                parameters.EntityPrefab = entityPrefab;
                return this;
            }
            public CommandParameters Build()
            {
                return parameters;
            }
        }
    }
}