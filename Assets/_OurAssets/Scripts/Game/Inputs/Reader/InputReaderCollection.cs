using System;
using System.Collections.Generic;
using CursedOnion.Game.Inputs;
using NaughtyAttributes;
using UnityEngine;

namespace CursedOnion.Game.Inputs
{
    [CreateAssetMenu(fileName = "InputMapCollection", menuName = "Game/Inputs/InputMapCollection")]
    public class InputReaderCollection : ScriptableObject
    {
        //TODO: Hacer el resto
        [Expandable, Required, SerializeField] BattleInputReader BattleInputReader;
        
        private Dictionary<Type, IInputReader> inputReaders;

        public void Initialize()
        {
            inputReaders = new();
            if(BattleInputReader != null) inputReaders.Add(typeof(BattleInputReader), BattleInputReader);
        }
        public T GetReader<T>() where T : IInputReader
        {
            return inputReaders.ContainsKey(typeof(T)) ? (T)inputReaders[typeof(T)] : default;
        }

        public void EnableReader<T>() where T : IInputReader
        {
            if (inputReaders.ContainsKey(typeof(T)))
            {
                inputReaders[typeof(T)]?.Enable();
            }
        }

        public void DisableAllReaders()
        {
            foreach (var reader in inputReaders.Values)
            {
                reader?.Disable();
            }
        }
        
    }
}
