using System;
using System.Threading.Tasks;
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
            SoundSettings.Initialize();
            
        }
        public async void SetSaveClients()
        {
            this.SaveClient ??= new CloudSaveClient();
            SoundSettings.SaveClient = SaveClient;
            LanguageSettings.SaveClient = SaveClient;
            ColorblindSettings.SaveClient = SaveClient;
            
            await Load();
            await Save();
        }
        public async Task Save()
        {
            if(!CloudUtils.CanUseCloud() || SaveClient == null) return;
            
            try
            {
                Debug.Log("[GameSettings]: Guardando...");
                
                await ColorblindSettings.Save();
                await SoundSettings.Save();
                await LanguageSettings.Save();
            }
            catch (Exception e)
            {
                Debug.LogWarning("Error al guardar: " + e);
            }
        }
        public async Task Load()
        {
            if(!CloudUtils.CanUseCloud()) return;
            
            try
            {
                await ColorblindSettings.Load();
                await SoundSettings.Load();
                await LanguageSettings.Load();
            }
            catch (Exception e)
            {
                Debug.LogWarning("Error al cargar: " + e);
            }
        }
    }
}
