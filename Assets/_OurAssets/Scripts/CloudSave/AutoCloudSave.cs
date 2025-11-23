using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CursedOnion.Game.Authentication;
using CursedOnion.Game.Settings;
using CursedOnion.Locators;
using Reflex.Attributes;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.Serialization;

namespace CursedOnion.Game.CloudSave
{
    public class AutoCloudSave : MonoBehaviour
    {
        [Inject] GameSettings gameSettings;
        [Inject] RuntimeVariableLocator variableLocator;
        
        [FormerlySerializedAs("debug")] [SerializeField] private bool autoLogIn;
        [SerializeField] private bool autoSave;
        
        CloudSaveClient saveClient;
        
        public event Action OnClientPrepared;
        async void Awake()
        {
            var instance = variableLocator.AutoCloudSave;
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
                variableLocator.AutoCloudSave = this;
                
                bool success = await GameAuthenticator.InitializeServices();

                if (success)
                {
                    AuthenticationService.Instance.SignedIn += PrepareClients;
                    AuthenticationService.Instance.Expired += TrySilentAuth;
                }

                if (autoLogIn)
                {
                    TrySilentAuth();
                }
            }
        }
        
        public async Task SaveGame()
        {
            if (CloudUtils.CanUseCloud())
            {
                var dataToSave = new Dictionary<string, object>();
                gameSettings.SaveInto(dataToSave);
                variableLocator.SaveInto(dataToSave);
                    
                await saveClient.Save(dataToSave);
            }
        }
        async void OnDisable()
        {
            try
            {
                var instance = variableLocator.AutoCloudSave;
                if (instance != null && instance == this)
                {
                    variableLocator.AutoCloudSave = null;

                    AuthenticationService.Instance.SignedIn -= PrepareClients;
                    AuthenticationService.Instance.Expired -= TrySilentAuth;
                
                    if (autoSave) await SaveGame();
                }
            }
            catch (Exception e)
            {
                throw; // TODO handle exception
            }
        }
        async void PrepareClients()
        {
            try
            {
                saveClient ??= new CloudSaveClient();

                if (CloudUtils.CanUseCloud())
                {
                    Debug.Log("Loading last saved data on cloud...");

                    var loadedData = await saveClient.LoadAll();
                    gameSettings.LoadFrom(loadedData);
                    variableLocator.LoadFrom(loadedData);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
            finally
            {
                OnClientPrepared?.Invoke();
            }
        }
        
        private async void TrySilentAuth()
        {
            try
            {
                await GameAuthenticator.TrySilentReAuth();
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
