using CursedOnion.Game.Cameras;
using UnityEngine;

namespace CursedOnion.Locators
{
    [CreateAssetMenu(fileName = "Runtime Variable Locator", menuName = "Game/Locators/Variable Locator")]
    public class RuntimeVariableLocator : ScriptableObject
    {
        public int LastLevelPlayed;
    }
}