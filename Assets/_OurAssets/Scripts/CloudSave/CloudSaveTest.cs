using System;
using System.Collections.Generic;
using CursedOnion.Locators;
using NaughtyAttributes;
using Reflex.Attributes;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;

namespace CursedOnion.Game.CloudSave
{
    public class CloudSaveTest : MonoBehaviour
    {
        [Inject] CloudSaveLocator cloudSaveLocator;
        private CloudSaveClient client;

        void Awake()
        {
            var instance = cloudSaveLocator.CloudSave;
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                AnonymousLogin();

                DontDestroyOnLoad(gameObject);
                cloudSaveLocator.CloudSave = this;
            }
        }

        public async void AnonymousLogin()
        {
            try
            {
                await UnityServices.InitializeAsync();

                if (!AuthenticationService.Instance.IsSignedIn) await AuthenticationService.Instance.SignInAnonymouslyAsync();

                Debug.Log("UGS inicializado correctamente. UserID: " + AuthenticationService.Instance.PlayerId);
                
                client = new CloudSaveClient();
            }
            catch (Exception e)
            {
                Debug.LogError("Error inicializando Cloud Save: " + e);
            }
        }
        
        [Button]
        public async void SaveTest()
        {
            try
            {
                await client.Save("one", "Just one string");
                Debug.Log("Datos guardados correctamente en Cloud");
            }
            catch (Exception e)
            {
                Debug.LogError("Error al guardar: " + e);
            }
            
        }
        
        [Button]
        public async void LoadTest()
        {
            try
            {
                var stringData = await client.Load<string>("one");
                Debug.Log("Datos cargados correctamente: " + stringData);
            }
            catch (Exception e)
            {
                Debug.LogError("Error al cargar: " + e);
            }
        }

        [Button]
        public async void DeleteTest()
        {
            try
            {
                await client.Delete("one");
                Debug.Log("Dato one borrados correctamente");
            }
            catch (Exception e)
            {
                Debug.LogError("Error al borrar: " + e);
            }
        }
        
        [Button]
        public async void DeleteAllTest()
        {
            try
            {
                await client.DeleteAll();
                Debug.Log("Datos borrados correctamente");
            }
            catch (Exception e)
            {
                Debug.LogError("Error al borrar todo: " + e);
            }
        }
    }
}
