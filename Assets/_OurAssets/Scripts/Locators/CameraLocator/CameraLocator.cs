using CursedOnion.Game.Cameras;
using UnityEngine;

namespace CursedOnion.Locators
{
    [CreateAssetMenu(fileName = "Camera Locator", menuName = "Game/Locators/Camera Locator")]
    public class CameraLocator : ScriptableObject
    {
        [System.NonSerialized] public GlobalCamera GlobalCamera;
    }
}
