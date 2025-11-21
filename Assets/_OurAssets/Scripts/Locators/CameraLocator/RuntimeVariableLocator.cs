using System;
using System.Threading.Tasks;
using CursedOnion.Game.Cameras;
using CursedOnion.Game.CloudSave;
using UnityEngine;

namespace CursedOnion.Locators
{
    [CreateAssetMenu(fileName = "Runtime Variable Locator", menuName = "Game/Locators/Variable Locator")]
    public class RuntimeVariableLocator : ScriptableObject, ICloudStorable
    {
        public CloudSaveClient SaveClient { get; set; }
        
        [System.NonSerialized] public AutoCloudSave AutoCloudSave;
        [System.NonSerialized] public GlobalCamera GlobalCamera;
        
        public int LastLevelPlayed;
        public bool IsGamePlayedOnMobile;
        
        public async void SetSaveClients()
        {
            this.SaveClient ??= new CloudSaveClient();
            await Load();
            await Save();
        }
        public async Task Save()
        {
            if(!CloudUtils.CanUseCloud() || SaveClient == null) return;
            
            try
            {

            }
            catch (Exception e)
            {
                Debug.LogWarning("Error al guardar: " + e);
            }
        }
        public async Task Load()
        {
            try
            {

            }
            catch (Exception e)
            {
                Debug.LogWarning("Error al cargar: " + e);
            }
        }
    }
}