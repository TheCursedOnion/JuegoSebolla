using UnityEngine;

namespace CursedOnion
{
    public interface IEntity
    {
        void DoTurn();
        void Attack(IEntity target);
        void Move();
    }
}
