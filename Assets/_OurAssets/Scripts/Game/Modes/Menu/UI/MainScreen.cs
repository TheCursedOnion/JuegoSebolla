using System;
using System.Threading.Tasks;
using CursedOnion.Game.Authentication;
using CursedOnion.Game.Settings;
using CursedOnion.Locators;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Modes.Menu.UI
{
    public class MainScreen : MonoBehaviour
    {
        [SerializeField] private GameObject logInScreen;
        [SerializeField] private GameObject signInScreen;
        [SerializeField] private GameObject mainScreen;
        
        [SerializeField] GameAuthenticator authenticator;
        
        [Inject] RuntimeVariableLocator variableLocator;
        async void Awake()
        {
            try
            {
                await authenticator.Initialize();
            
                if (!authenticator.HasSignedIn)
                {
                    EnterLogInScreen();
                }
                else
                {
                    EnterMainScreen();
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }

        public void EnterLogInScreen()
        {
            DisableAllScreens();
            logInScreen.SetActive(true);
        }
        public void EnterSignInScreen()
        {
            DisableAllScreens();
            signInScreen.SetActive(true);
        }
        public void EnterMainScreen()
        {
            DisableAllScreens();
            mainScreen.SetActive(true);
        }
        public Task SignInAnonymously()
        {
            return ProcessSignIn(() => authenticator.AnonymousLogin());
        }

        public Task SignIn(string username, string password)
        {
            return ProcessSignIn(() => authenticator.LoginUser(username, password));
        }
        private async Task ProcessSignIn(Func<Task<AuthResult>> loginFunc)
        {
            try
            {
                var result = await loginFunc();
                if (result.Success)
                {
                    DisableAllScreens();
                    mainScreen.SetActive(true);
                    variableLocator.InvokeSignIn();
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }
        
        void DisableAllScreens()
        {
            logInScreen.SetActive(false);
            signInScreen.SetActive(false);
            mainScreen.SetActive(false);
        }
    }
}
