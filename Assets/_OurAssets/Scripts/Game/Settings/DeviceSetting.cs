using System;
using UnityEngine.InputSystem;

namespace CursedOnion.Game.Settings
{
    [Serializable]
    public class DeviceSetting
    {
        public InputDevice CurrentDevice;
        //TODO: Device Config classes
        
        public Action<InputDevice> OnChange { get; set; }
    }
}