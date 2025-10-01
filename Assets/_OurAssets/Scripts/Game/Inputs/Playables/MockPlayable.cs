using System;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CursedOnion.Game.Inputs
{
    public class MockPlayable : MonoBehaviour, IPlayable
    {
        [Inject] public InputReader InputReader { get; set; }
        [field: SerializeField] public string UsedMap { get; set; }
        

        public void OnEnable()
        {
            if(InputReader == null) return;
            
            InputAction selectAction = InputReader.FindMapAction(UsedMap,"Select");
            selectAction.performed += Test;
        }

        public void OnDisable()
        {
            if(InputReader == null) return;
            
            InputAction selectAction = InputReader.FindMapAction(UsedMap,"Select");
            selectAction.performed -= Test;
        }

        void Test(InputAction.CallbackContext context)
        {
            Debug.Log("Hola");
        }
    }
}