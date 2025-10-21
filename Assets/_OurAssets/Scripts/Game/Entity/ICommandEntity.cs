using UnityEngine;

namespace CursedOnion.Game.Entity
{
    public interface IEntity
    {
        public string Name { get; set; }
        public Transform Transform { get; set; }
        public EntityData Data { get; set; }
        public EntityStats Stats { get; set; }
        public EntityFlags Flags { get; set; }
    }
    public interface ICommandEntity : IEntity
    {
        void DoTurn();
        void Attack(IEntity target);
        void Move(Vector3 newPosition);
    }
}
