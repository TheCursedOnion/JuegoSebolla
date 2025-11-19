using CursedOnion.Game.CloudSave;
using UnityEngine;

namespace CursedOnion.Locators
{
    [CreateAssetMenu(fileName = "CloudSave Locator", menuName = "Game/Locators/CloudSave Locator")]
    public class CloudSaveLocator : ScriptableObject
    {
        [System.NonSerialized] public CloudSaveTest CloudSave;
    }
}