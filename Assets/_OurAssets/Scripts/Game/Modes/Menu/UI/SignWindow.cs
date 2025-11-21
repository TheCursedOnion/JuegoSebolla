using System;
using CursedOnion.Game.Authentication;
using CursedOnion.Game.Localization;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;

namespace CursedOnion.Game.Modes.Menu.UI
{
    public class SignWindow : MonoBehaviour
    {
        [SerializeField] MainScreen mainScreen;
        
        [SerializeField] TMP_InputField usernameInput;
        [SerializeField] TMP_InputField passwordInput;
        
        [SerializeField] LocalizedGUIText errorText;

        private void OnEnable()
        {
            errorText.SetNullKey();
            usernameInput.text = "";
            passwordInput.text = "";
        }

        public async void SignUp()
        {
            Debug.Log("Sign up");
            string username = usernameInput.text;
            string password = passwordInput.text;

            var result = await GameAuthenticator.RegisterUser(username, password);
            if (!result.Success)
            {
                Debug.LogError(result.Error);
                LogError(result.Error);
            }
        }
        
        public async void LogIn()
        {
            string username = usernameInput.text;
            string password = passwordInput.text;

            var result = await GameAuthenticator.LoginUser(username, password);
            if (!result.Success)
            {
                Debug.LogError(result.Error);
                LogError(result.Error);
            }
        }

        public async void AnonymousSignIn()
        {
            var result = await GameAuthenticator.AnonymousLogin();
            if (!result.Success)
            {
                Debug.LogError(result.Error);
                LogError(result.Error);
            }
        }

        void LogError(int error)
        {
            switch (error)
            {
                case 1: //InvalidUsername or InvalidPassword
                case 2: //InvalidUsername and InvalidPassword
                    errorText.SetKey("auth_error_credentials");
                    break;
                case 3: //AlreadyRegistered
                    errorText.SetKey("auth_error_already_registered");
                    break;
                default:
                    errorText.SetKey("auth_error_unknown");
                    break;
            }
        }
    }
}
