using System;
using UnityEngine.InputSystem;

namespace CursedOnion.Game.Settings
{
    [Serializable]
    public class DeviceSetting : ISetting<DeviceSetting>
    {
        public InputDevice CurrentDevice;
        //TODO: Device Config classes
        
        public Action<DeviceSetting> OnChange { get; set; }
    }
}