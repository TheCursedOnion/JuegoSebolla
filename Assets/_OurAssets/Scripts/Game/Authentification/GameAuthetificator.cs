using System;
using System.Threading.Tasks;
using CursedOnion.Game.CloudSave;
using CursedOnion.Game.Inputs;
using CursedOnion.Game.Settings;
using NaughtyAttributes;
using Reflex.Attributes;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace CursedOnion.Game.Authentification
{
    public class GameAuthetificator : MonoBehaviour
    {
        public async Task<AuthResult<string>> AnonymousLogin()
        {
            if(AuthenticationService.Instance.IsSignedIn) return new AuthResult<string> { Error = AuthError.AlreadySignedIn };
            try
            {
                await UnityServices.InitializeAsync();

                if (!AuthenticationService.Instance.IsSignedIn)
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                
                Debug.Log("UGS inicializado correctamente. UserID: " + AuthenticationService.Instance.PlayerId);
                return new AuthResult<string> { Value = "OK" };
            }
            catch (Exception ex)
            {
                Debug.LogError("Error inicializando Cloud Save: " + ex);
                return new AuthResult<string> { Error = AuthExceptionHandler.MapExceptionToAuthError(ex) };
            }
        }
        
        public async Task<AuthResult<string>> RegisterUser(string username, string password)
        {
            try
            {
                await UnityServices.InitializeAsync();
                
                await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
                Debug.Log("Usuario registrado: " + AuthenticationService.Instance.PlayerId);
                return new AuthResult<string> { Value = "OK" };
            }
            catch (Exception ex)
            {
                return new AuthResult<string> { Error = AuthExceptionHandler.MapExceptionToAuthError(ex) };
            }
        }
        public async Task<AuthResult<string>> LoginUser(string username, string password)
        {
            if(AuthenticationService.Instance.IsSignedIn) return new AuthResult<string> { Error = AuthError.AlreadySignedIn };
            try
            {
                await UnityServices.InitializeAsync();
                
                await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
                Debug.Log("Login correcto: " + AuthenticationService.Instance.PlayerId);
                return new AuthResult<string> { Value = "OK" };
            }
            catch (Exception ex)
            {
                Debug.LogError("Error en login: " + ex.Message);
                return new AuthResult<string> { Error = AuthExceptionHandler.MapExceptionToAuthError(ex) };
            }
        }
    }
}
