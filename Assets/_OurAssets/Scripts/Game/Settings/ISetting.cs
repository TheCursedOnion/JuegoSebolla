using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CursedOnion.Game.Settings
{
    public interface ISetting<T>
    {
        public Action<T> OnChange {get; set;}
    }
    
    [Serializable]
    public class VolumeSetting : ISetting<VolumeSetting>
    {
        [SerializeField] float sfxVolume;
        
        public Action<VolumeSetting> OnChange { get; set; }
    }
    
    [Serializable]
    public class DeviceSetting : ISetting<DeviceSetting>
    {
        public InputDevice CurrentDevice;
        //TODO: Device Config classes
        
        public Action<DeviceSetting> OnChange { get; set; }
    }
}