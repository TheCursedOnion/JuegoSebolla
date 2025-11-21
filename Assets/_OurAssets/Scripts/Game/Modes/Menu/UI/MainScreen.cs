using System;
using System.Threading.Tasks;
using CursedOnion.Game.Authentication;
using CursedOnion.Game.Settings;
using CursedOnion.Locators;
using Reflex.Attributes;
using Unity.Services.Authentication;
using UnityEngine;

namespace CursedOnion.Game.Modes.Menu.UI
{
    public class MainScreen : MonoBehaviour
    {
        [SerializeField] private GameObject logInScreen;
        [SerializeField] private GameObject signUpScreen;
        [SerializeField] private GameObject mainScreen;
        
        [Inject] RuntimeVariableLocator variableLocator;
        async void Awake()
        {
            try
            {
                await GameAuthenticator.InitializeServices();
                
                if (!GameAuthenticator.HasSignedIn)
                {
                    EnterLogInScreen();
                }
                else
                {
                    EnterMainScreen();
                }
                
                AuthenticationService.Instance.SignedIn += EnterMainScreen;
                AuthenticationService.Instance.Expired += EnterSignUpScreen;
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }
        void OnDisable()
        {
            AuthenticationService.Instance.SignedIn -= EnterMainScreen;
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
        void DisableAllScreens()
        {
            logInScreen.SetActive(false);
            signUpScreen.SetActive(false);
            mainScreen.SetActive(false);
        }
    }
}
