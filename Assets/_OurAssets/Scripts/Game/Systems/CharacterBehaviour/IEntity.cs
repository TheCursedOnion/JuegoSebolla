using UnityEngine;

namespace CursedOnion
{
    public interface IEntity
    {
        void DoTurn();
        void Attack();
        void Move();
    }
}
