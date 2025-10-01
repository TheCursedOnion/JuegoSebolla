using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CursedOnion.Game.Inputs
{
    public class InputReader : MonoBehaviour
    {
        [Required, SerializeField] PlayerInput playerInput;
        public PlayerInput PlayerInput => playerInput;

        #region Action Related

        public InputAction FindMapAction(string map, string actionName)
        {
            return playerInput.actions.FindActionMap(map).FindAction(actionName);
        }
        

        #endregion

        #region Map Related
        public Action<string> OnMapChange { get; set; }
        Stack<string> inputMapStack = new Stack<string>();
        
        public void SetMapActive(string mapName, bool active)
        {
            InputActionMap map = playerInput.actions.FindActionMap(mapName);
            
            if(active)
                map.Enable();
            else
                map.Disable();
        }
        
        public void PushInputMap(string inputMapName)
        {
            if(inputMapStack.Count > 0 && inputMapStack.Peek() == inputMapName) return;
            
            inputMapStack.Push(inputMapName);
            SwitchPlayerInputMap(inputMapName);
        }
        public void PopInputMap()
        {
            if(inputMapStack.Count == 1) return;
            
            inputMapStack.Pop();
            SwitchPlayerInputMap(inputMapStack.Peek());
        }

        void SwitchPlayerInputMap(string mapModeName)
        {
            playerInput.SwitchCurrentActionMap(mapModeName);
            OnMapChange?.Invoke(mapModeName);
        }
        #endregion
        
    }
}
