using System;
using System.Collections.Generic;
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
        [SerializeField] bool autoSave;
        
        [Inject] GameSettings gameSettings;
        [Inject] RuntimeVariableLocator variableLocator;
        private CloudSaveClient client;

        void Awake()
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

                variableLocator.OnSignIn += InsertAuthentications;
            }
        }

        void InsertAuthentications()
        {
            client = new CloudSaveClient();
            //variableLocator.SetSaveClients(client);
            gameSettings.SetSaveClients(client);
        }
        
        void OnDisable()
        {
            if (!autoSave) return;
            
            var instance = variableLocator.AutoCloudSave;
            if (instance != null && instance == this)
            {
                variableLocator.OnSignIn -= InsertAuthentications;
                
                variableLocator.AutoCloudSave = null;
                _ = gameSettings.Save();
                _ = variableLocator.Save();
            }
        }
    }
}
