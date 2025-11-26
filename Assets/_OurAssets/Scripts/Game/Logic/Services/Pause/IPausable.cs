using UnityEngine;

namespace CursedOnion.Game.Logic.Services.Pause
{
    public interface IPausable
    {
        public void Pause(PauseLevel level);
        public void Unpause();
    }
}
