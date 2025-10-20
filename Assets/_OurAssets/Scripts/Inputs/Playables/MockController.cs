using System;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CursedOnion.Game.Inputs
{
    public class MockController : MonoBehaviour, IController
    {
        [Inject] public InputReaderCollection InputReaderCollection { get; set; }

        private void OnEnable() => Enable();
        public void Enable()
        {
            TileSelectorInputReader reader = InputReaderCollection.GetReader<TileSelectorInputReader>();
            reader.Select += Test;
        }
        
        private void OnDisable() => Disable();
        public void Disable()
        {
            TileSelectorInputReader reader = InputReaderCollection.GetReader<TileSelectorInputReader>();
            reader.Select += Test;
        }

        void Test()
        {
            Debug.Log("Hola");
        }
    }
}