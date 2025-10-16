using CursedOnion.Game.Cameras;
using UnityEngine;

namespace CursedOnion.Game.Settings
{
    [CreateAssetMenu(fileName = "RuntimeSettings", menuName = "Game/Settings/Scriptable Runtime Settings")]
    public class RuntimeSettings : ScriptableObject
    {
        [System.NonSerialized] public GlobalCamera GlobalCamera;
    }
}
