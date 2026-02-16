using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace CursedOnion.Game.Authentication
{
    public enum AuthMode { None = 0, Anonymous = 1, UserPass = 2 }
    public static class GameAuthenticator
    {
        private const string KEY_USERNAME = "auth_username";
        private const string KEY_PASSWORD = "auth_password";
        private const string KEY_AUTH_MODE = "auth_mode";
        
        public static AuthMode CurrentMode = AuthMode.None;

        public static bool HasSignedIn => UnityServices.Instance.State == ServicesInitializationState.Initialized
                                            && AuthenticationService.Instance.IsSignedIn;
        private static void SaveAuthCredentials(string username, string password)
        {
            PlayerPrefs.SetString(KEY_USERNAME, username);
            PlayerPrefs.SetString(KEY_PASSWORD, password);
            PlayerPrefs.SetInt(KEY_AUTH_MODE, (int)AuthMode.UserPass);
            PlayerPrefs.Save();

            CurrentMode = AuthMode.UserPass;
        }
        private static void SaveAuthAnonymous()
        {
            PlayerPrefs.SetInt(KEY_AUTH_MODE, (int)AuthMode.Anonymous);
            PlayerPrefs.Save();

            CurrentMode = AuthMode.Anonymous;
        }
        public static async Task<AuthResult> TrySilentReAuth()
        {
            Debug.LogWarning("Auth expired — attempting auto re-auth…");

            int authMode = PlayerPrefs.GetInt(KEY_AUTH_MODE, -1);
            
            Debug.Log($"Auth mode: {authMode}");
            if (authMode == -1) return new AuthResult { Error = -1 };
            
            CurrentMode = (AuthMode)authMode;
            switch (CurrentMode)
            {
                case AuthMode.UserPass:
                    string user = PlayerPrefs.GetString(KEY_USERNAME, "");
                    string pass = PlayerPrefs.GetString(KEY_PASSWORD, "");
                    
                    if (IsValidUsername(user) && IsValidPassword(pass))
                        return await LoginUser(user, pass);

                    break;

                case AuthMode.Anonymous:
                    return await AnonymousLogin();
            }
            return new AuthResult { Error = -1 };
        }
        
        public static async Task<bool> InitializeServices()
        {
            if (UnityServices.State == ServicesInitializationState.Uninitialized)
            {
                try
                {
                    await UnityServices.InitializeAsync();
                    return true;
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to initialize Unity Services: {e}");
                    return false;
                }
            }
            return false;
        }
        public static async Task<AuthResult> AnonymousLogin()
        {
            if(AuthenticationService.Instance.IsSignedIn) return new AuthResult { Error = AuthenticationErrorCodes.ClientNoActiveSession };
            try
            {
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                    SaveAuthAnonymous();
                }

                Debug.Log("UGS inicializado correctamente. UserID: " + AuthenticationService.Instance.PlayerId);
                return new AuthResult();
            }
            catch (AuthenticationException authEx)
            {
                Debug.Log("Error auth al loggear usuario anon. " + authEx.Message);
                return new AuthResult { Error = AuthExceptionHandler.MapExceptionToAuthError(authEx) };
            }
            catch (RequestFailedException reqEx)
            {
                Debug.Log("Error request al loggear usuario anon. " + reqEx.Message);
                return new AuthResult { Error = AuthExceptionHandler.MapExceptionToAuthError(reqEx) };
            }
            catch (Exception ex)
            {
                Debug.Log("Error desconocido al loggear usuario anon. " + ex.Message);
                return new AuthResult { Error = -1 };
            }
        }
        
        public static async Task<AuthResult> RegisterUser(string username, string password)
        {
            try
            {
                Debug.Log("Intentando registrar usuario...");
                await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
                Debug.Log("Usuario registrado: " + AuthenticationService.Instance.PlayerId);
                return new AuthResult();
            }
            catch (AuthenticationException authEx)
            {
                Debug.Log("Error auth al registrar usuario. " + authEx.Message);
                return new AuthResult { Error = AuthExceptionHandler.MapExceptionToAuthError(authEx) };
            }
            catch (RequestFailedException reqEx)
            {
                Debug.Log("Error request al registrar usuario. " + reqEx.Message);
                return new AuthResult { Error = AuthExceptionHandler.MapExceptionToAuthError(reqEx) };
            }
            catch (Exception ex)
            {
                Debug.Log("Error desconocido al registrar usuario. " + ex.Message);
                return new AuthResult { Error = -1 };
            }
        }
        public static async Task<AuthResult> LoginUser(string username, string password)
        {

            try
            {
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
                    SaveAuthCredentials(username, password);
                }

                Debug.Log("Login correcto: " + AuthenticationService.Instance.PlayerId);
                return new AuthResult();
            }
            catch (AuthenticationException authEx)
            {
                Debug.Log("Error auth al loggear usuario. " + authEx.Message);
                return new AuthResult { Error = AuthExceptionHandler.MapExceptionToAuthError(authEx) };
            }
            catch (RequestFailedException reqEx)
            {
                Debug.Log("Error request al loggear usuario. " + reqEx.Message);
                return new AuthResult { Error = AuthExceptionHandler.MapExceptionToAuthError(reqEx) };
            }
            catch (Exception ex)
            {
                Debug.Log("Error desconocido al loggear usuario. " + ex.Message);
                return new AuthResult { Error = -1 };
            }
        }
        
        public static bool IsValidUsername(string username)
        {
            return !string.IsNullOrEmpty(username) && username.Length > 3 && username.Length < 20;
        }
        public static bool IsValidPassword(string password)
        {
            if(string.IsNullOrEmpty(password) || password.Length < 8 || password.Length > 30) return false;
            
            bool hasLowercase = false;
            bool hasUppercase = false;
            bool hasDigit = false;
            bool hasSymbol = false;
            
            foreach (char c in password)
            {
                if (char.IsLower(c)) hasLowercase = true;
                else if (char.IsUpper(c)) hasUppercase = true;
                else if (char.IsDigit(c)) hasDigit = true;
                else if (!char.IsLetterOrDigit(c)) hasSymbol = true;
            }
            return hasLowercase && hasUppercase && hasDigit && hasSymbol;
        }
        public static async Task UpdatePasswordAsync(string currentPassword, string newPassword)
        {
            try
            {
                await AuthenticationService.Instance.UpdatePasswordAsync(currentPassword, newPassword);
                Debug.Log("Password updated.");
            }
            catch (AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
            catch (RequestFailedException ex)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
        }
    }
}
