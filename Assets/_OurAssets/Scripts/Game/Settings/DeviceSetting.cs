using System;
using UnityEngine.InputSystem;

namespace CursedOnion.Game.Settings
{
    [Serializable]
    public class DeviceSetting : ISetting<InputDevice>
    {
        public InputDevice CurrentDevice;
        //TODO: Device Config classes
        
        public Action<InputDevice> OnChange { get; set; }
    }
}