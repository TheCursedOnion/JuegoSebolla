using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace CursedOnion.Game.Authentication
{
    public class GameAuthenticator : MonoBehaviour
    {
        public async Task Initialize()
        {
            if (UnityServices.State == ServicesInitializationState.Uninitialized)
            {
                try
                {
                    await UnityServices.InitializeAsync();
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to initialize Unity Services: {e}");
                }
            }
        }
        public bool HasSignedIn => AuthenticationService.Instance.IsSignedIn;
        public async Task<AuthResult> AnonymousLogin()
        {
            if(AuthenticationService.Instance.IsSignedIn) return new AuthResult { Error = AuthError.AlreadySignedIn };
            try
            {
                await UnityServices.InitializeAsync();

                if (!AuthenticationService.Instance.IsSignedIn)
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                
                Debug.Log("UGS inicializado correctamente. UserID: " + AuthenticationService.Instance.PlayerId);
                return new AuthResult();
            }
            catch (Exception ex)
            {
                Debug.LogError("Error inicializando Cloud Save: " + ex);
                return new AuthResult { Error = AuthExceptionHandler.MapExceptionToAuthError(ex) };
            }
        }
        
        public async Task<AuthResult> RegisterUser(string username, string password)
        {
            try
            {
                await UnityServices.InitializeAsync();
                
                await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
                Debug.Log("Usuario registrado: " + AuthenticationService.Instance.PlayerId);
                return new AuthResult();
            }
            catch (Exception ex)
            {
                return new AuthResult { Error = AuthExceptionHandler.MapExceptionToAuthError(ex) };
            }
        }
        public async Task<AuthResult> LoginUser(string username, string password)
        {
            if(AuthenticationService.Instance.IsSignedIn) return new AuthResult { Error = AuthError.AlreadySignedIn };
            try
            {
                await UnityServices.InitializeAsync();
                
                await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
                Debug.Log("Login correcto: " + AuthenticationService.Instance.PlayerId);
                return new AuthResult();
            }
            catch (Exception ex)
            {
                Debug.LogError("Error en login: " + ex.Message);
                return new AuthResult { Error = AuthExceptionHandler.MapExceptionToAuthError(ex) };
            }
        }
    }
}
