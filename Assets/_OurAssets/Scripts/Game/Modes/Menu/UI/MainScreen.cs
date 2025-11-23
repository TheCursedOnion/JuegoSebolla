using System;
using System.Threading.Tasks;
using CursedOnion.Game.Authentication;
using CursedOnion.Game.Settings;
using CursedOnion.Locators;
using Reflex.Attributes;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace CursedOnion.Game.Modes.Menu.UI
{
    public class MainScreen : MonoBehaviour
    {
        [Inject] RuntimeVariableLocator variableLocator;
        [SerializeField] private GameObject logInScreen;
        [SerializeField] private GameObject signUpScreen;
        [SerializeField] private GameObject mainScreen;
        void Awake()
        {
            UnityServices.Initialized += OnServicesInitialized;
            UpdateScreen();
        }
        
        void OnServicesInitialized()
        {
            OnDisable();

            var cloudSave = variableLocator.AutoCloudSave;
            if(cloudSave != null) cloudSave.OnClientPrepared += EnterMainScreen;
            
            AuthenticationService.Instance.SignedOut += EnterLogInScreen;
            AuthenticationService.Instance.Expired += EnterSignUpScreen;
            
            UpdateScreen();
        }
        void OnDisable()
        {
            var cloudSave = variableLocator.AutoCloudSave;
            if(cloudSave != null) cloudSave.OnClientPrepared -= EnterMainScreen;
            
            AuthenticationService.Instance.SignedOut -= EnterLogInScreen;
            AuthenticationService.Instance.Expired -= EnterSignUpScreen;
        }
        public void EnterLogInScreen()
        {
            DisableAllScreens();
            logInScreen.SetActive(true);
        }
        public void EnterSignUpScreen()
        {
            DisableAllScreens();
            signUpScreen.SetActive(true);
        }
        public void EnterMainScreen()
        {
            DisableAllScreens();
            mainScreen.SetActive(true);
        }
        void UpdateScreen()
        {
            if (GameAuthenticator.HasSignedIn)
                EnterMainScreen();
            else
                EnterLogInScreen();
        }
        void DisableAllScreens()
        {
            logInScreen.SetActive(false);
            signUpScreen.SetActive(false);
            mainScreen.SetActive(false);
        }
    }
}
