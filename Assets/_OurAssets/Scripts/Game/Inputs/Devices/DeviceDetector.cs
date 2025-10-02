using CursedOnion.Game.Inputs;
using CursedOnion.Game.Settings;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace CursedOnion.Game.Inputs
{
    [RequireComponent(typeof(PlayerInput))]
    public class DeviceDetector : MonoBehaviour
    {
        [Inject] GameSettings gameSettings;
        
        private PlayerInput playerInput;
        void Awake()
        {
            DontDestroyOnLoad(this.gameObject.transform.root.gameObject);
            playerInput = GetComponent<PlayerInput>();
        }
        
        public void OnControlsChanged(PlayerInput obj)
        {
            var device = obj.devices[0];
            gameSettings.DeviceSettings.CurrentDevice = device;
            Debug.Log($"Dispositivo cambiado a: {device.displayName} ({device.deviceId})");
        }
        
    }
}
