using System;
using System.Collections.Generic;
using CursedOnion.Game.Cameras;
using CursedOnion.Game.CloudSave;
using Unity.Services.CloudSave.Models;
using UnityEngine;

namespace CursedOnion.Locators
{
    [CreateAssetMenu(fileName = "Runtime Variable Locator", menuName = "Game/Locators/Variable Locator")]
    public class RuntimeVariableLocator : ScriptableObject, ICloudStorable
    {
        
        [System.NonSerialized] public AutoCloudSave AutoCloudSave;
        [System.NonSerialized] public GlobalCamera GlobalCamera;
        
        public int LastLevelPlayed;
        public bool IsGamePlayedOnMobile;
        
        public void SaveInto(Dictionary<string, object> serializableData)
        {
            
        }
        public void LoadFrom(Dictionary<string, Item> loadedData)
        {
            
        }
    }
}