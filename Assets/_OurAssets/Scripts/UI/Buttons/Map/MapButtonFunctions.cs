using CursedOnion.Game.Objects;
using CursedOnion.Helpers;
using CursedOnion.Locators;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.UI
{
    public class MapButtonFunctions : MonoBehaviour
    {
        [Inject] MapManager mapManager;

        public void MoveToNextLevel()
        {
            mapManager.MoveToNextLevel();
        }

        public void MoveToPreviousLevel()
        {
            mapManager.MoveToPreviousLevel();
        }
    }
}
