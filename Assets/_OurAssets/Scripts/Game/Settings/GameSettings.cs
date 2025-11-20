using System;
using CursedOnion.Game.CloudSave;
using UnityEngine;

namespace CursedOnion.Game.Settings
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Game/Settings/Scriptable Settings")]
    public class GameSettings : ScriptableObject, ICloudStorable
    {
        public CloudSaveClient SaveClient { get; set; }
        
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
        public void InitializeSettings()
        {
            LanguageSettings.Initialize();
        }
        public async void Save()
        {
            try
            {
                ColorblindSettings.Save();
                SoundSettings.Save();
                LanguageSettings.Save();
            }
            catch (Exception e)
            {
                Debug.LogError("Error al guardar: " + e);
            }
        }
        public async void Load()
        {
            try
            {
                ColorblindSettings.Load();
                SoundSettings.Load();
                LanguageSettings.Load();
            }
            catch (Exception e)
            {
                Debug.LogWarning("Error al cargar: " + e);
            }
        }

        public async void LoadLastSavedData()
        {
            void SetSaveClients()
            {
                this.SaveClient ??= new CloudSaveClient();
                SoundSettings.SaveClient = SaveClient;
                LanguageSettings.SaveClient = SaveClient;
                ColorblindSettings.SaveClient = SaveClient;
            }
        }
    }
}
