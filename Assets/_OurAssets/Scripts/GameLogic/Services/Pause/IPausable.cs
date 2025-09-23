using UnityEngine;

namespace CursedOnion.GameLogic.Services.Pause
{
    public interface IPausable
    {
        public void Pause();
        public void Unpause();
    }
}
