using System;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CursedOnion.Game.Inputs
{
    public class MockPlayable : MonoBehaviour, IPlayable
    {
        [Inject] public InputReaderCollection InputReaderCollection { get; set; }

        public void OnEnable()
        {
            BattleInputReader reader = InputReaderCollection.GetReader<BattleInputReader>();
            reader.Select += Test;
        }

        public void OnDisable()
        {
            BattleInputReader reader = InputReaderCollection.GetReader<BattleInputReader>();
            reader.Select += Test;
        }

        void Test()
        {
            Debug.Log("Hola");
        }
    }
}