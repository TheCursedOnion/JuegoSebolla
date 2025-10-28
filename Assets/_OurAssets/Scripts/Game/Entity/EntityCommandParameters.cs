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
}