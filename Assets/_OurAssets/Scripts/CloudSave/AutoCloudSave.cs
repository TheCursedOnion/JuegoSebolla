using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CursedOnion.Game.Authentication;
using CursedOnion.Game.Settings;
using CursedOnion.Locators;
using NaughtyAttributes;
using Reflex.Attributes;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;

namespace CursedOnion.Game.CloudSave
{
    public class AutoCloudSave : MonoBehaviour
    {
        [SerializeField] private bool debug;
        [SerializeField] private bool autoSave;
        [Inject] GameSettings gameSettings;
        [Inject] RuntimeVariableLocator variableLocator;
        
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
                
                await GameAuthenticator.InitializeServices();
                AuthenticationService.Instance.SignedIn += PrepareClients;
                AuthenticationService.Instance.Expired += OnAuthExpired;

                if (debug)
                {
                    await GameAuthenticator.AnonymousLogin();
                }
            }
        }
        void OnDisable()
        {
            var instance = variableLocator.AutoCloudSave;
            if (instance != null && instance == this)
            {
                variableLocator.AutoCloudSave = null;

                if (autoSave)
                {
                    _ = gameSettings.Save();
                    _ = variableLocator.Save();
                }

                AuthenticationService.Instance.SignedIn -= PrepareClients;
                AuthenticationService.Instance.Expired -= OnAuthExpired;
            }
        }
        void PrepareClients()
        {
            gameSettings.SetSaveClients();
            variableLocator.SetSaveClients();
        }
        private async void OnAuthExpired()
        {
            try
            {
                await GameAuthenticator.TrySilentReAuth();
            }
            catch (Exception e)
            {
                throw; // TODO handle exception
            }
        }
    }
}
