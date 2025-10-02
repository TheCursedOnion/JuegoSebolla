using UnityEngine;

namespace CursedOnion.Game.Settings
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Game/Settings/Scriptable Settings")]
    public class GameSettings : ScriptableObject
    {
        public VolumeSetting VolumeSettings;
        public DeviceSetting DeviceSettings;
    }
}
