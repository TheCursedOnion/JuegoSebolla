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
        const string LAST_LEVEL_KEY = "LastLevelPlayed";
        const string COMPLETED_LEVELS = "CompletedLevels";

        [System.NonSerialized] public AutoCloudSave AutoCloudSave;
        [System.NonSerialized] public GlobalCamera GlobalCamera;
        
        public int LastPlayedLevel;
        public int LastCompletedLevel;
        public bool IsGamePlayedOnMobile;
        
        public void SetCompletedLevel(int levelIndex)
        {
            if (levelIndex > LastCompletedLevel)
            {
                LastCompletedLevel = levelIndex;
                _ = AutoCloudSave?.SaveGame();
            }
        }
        public void SaveInto(Dictionary<string, object> serializableData)
        {
            serializableData[LAST_LEVEL_KEY] = LastPlayedLevel;
            serializableData[COMPLETED_LEVELS] = LastCompletedLevel;
        }
        public void LoadFrom(Dictionary<string, Item> loadedData)
        {
            int lastLevel = CloudUtils.GetValueFromQuery(loadedData, LAST_LEVEL_KEY, 0);
            LastPlayedLevel = lastLevel;
            
            lastLevel = CloudUtils.GetValueFromQuery(loadedData, LAST_LEVEL_KEY, -1);
            LastCompletedLevel = lastLevel;
        }
    }
}