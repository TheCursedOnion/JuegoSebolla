using System;
using System.Collections.Generic;
using CursedOnion.Game.CloudSave;
using Unity.Services.CloudSave.Models;
using UnityEngine;

namespace CursedOnion.Game.Settings
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Game/Settings/Scriptable Settings")]
    public class GameSettings : ScriptableObject, ICloudStorable
    {
        
        [System.NonSerialized] GlobalVolume globalVolume;
        public DeviceSetting DeviceSettings;
        public SoundSetting SoundSettings;
        public LanguageSetting LanguageSettings;
        public ColorblindSetting ColorblindSettings;
        
        public GlobalVolume GetGlobalVolume() => globalVolume;
        public void SetGlobalVolume(GlobalVolume globalVolume)
        {
            this.globalVolume = globalVolume;
            ColorblindSettings.SetGlobalVolume(globalVolume);
        }
        public void Initialize()
        {
            LoadDefaultSettings();
        }
        public void SaveInto(Dictionary<string, object> serializableData)
        {
            Debug.Log("[GameSettings]: Guardando...");
            try
            {
                ColorblindSettings.SaveInto(serializableData);
                SoundSettings.SaveInto(serializableData);
                LanguageSettings.SaveInto(serializableData);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
            
        }

        public void LoadDefaultSettings()
        {
            LoadFrom(null);
        }
        public void LoadFrom(Dictionary<string, Item> loadedData)
        {
            Debug.Log("[GameSettings]: Cargando...");
            
            ColorblindSettings.LoadFrom(loadedData);
            SoundSettings.LoadFrom(loadedData);
            LanguageSettings.LoadFrom(loadedData);
        }
    }
}
