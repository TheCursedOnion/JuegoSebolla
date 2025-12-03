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
        const string LAST_DIALOG_KEY = "LastDialogCompleted";
        const string LAST_LEVEL_KEY = "LastLevelPlayed";
        const string COMPLETED_LEVELS = "CompletedLevels";

        [System.NonSerialized] public AutoCloudSave AutoCloudSave;
        [System.NonSerialized] public GlobalCamera GlobalCamera;
        
        public int LastDialogCompleted;
        public int LastPlayedLevel;
        public int LastCompletedLevel;
        public bool IsGamePlayedOnMobile;
        
        
        public void SetLastDialogCompleted(int dialogIndex)
        {
            if (dialogIndex > LastDialogCompleted)
            {
                LastDialogCompleted = dialogIndex;
                _ = AutoCloudSave?.SaveGame();
            }
        }
        public void SetLastPlayedLevel(int levelIndex)
        {
            LastPlayedLevel = levelIndex;
        }
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
            serializableData[LAST_DIALOG_KEY] = LastDialogCompleted;
            serializableData[LAST_LEVEL_KEY] = LastPlayedLevel;
            serializableData[COMPLETED_LEVELS] = LastCompletedLevel;
        }
        public void LoadFrom(Dictionary<string, Item> loadedData)
        {
            int lastInt = CloudUtils.GetValueFromQuery(loadedData, LAST_DIALOG_KEY, -1);
            LastDialogCompleted = lastInt;
            
            lastInt = CloudUtils.GetValueFromQuery(loadedData, LAST_LEVEL_KEY, 0);
            LastPlayedLevel = lastInt;
            
            lastInt = CloudUtils.GetValueFromQuery(loadedData, COMPLETED_LEVELS, -1);
            LastCompletedLevel = lastInt;
        }
    }
}