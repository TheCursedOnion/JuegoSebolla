using System;
using Unity.Services.Core;

namespace CursedOnion.Game.Authentication
{
    public enum AuthError
    {
        None,
        AlreadySignedIn,
        NetworkError,
        InvalidCredentials,
        AlreadyExists,
        Unknown
    }
    public static class AuthExceptionHandler
    {
        public static AuthError MapExceptionToAuthError(Exception ex)
        {
            if (ex.Message.Contains("401") || ex.Message.Contains("Invalid"))
                return AuthError.InvalidCredentials;

            if (ex.Message.Contains("AlreadyExists"))
                return AuthError.AlreadyExists;

            if (ex is RequestFailedException rfe && rfe.Reason == 0) // Sin red
                return AuthError.NetworkError;

            return AuthError.Unknown;
        }
    }
    public struct AuthResult
    {
        public bool Success => Error == AuthError.None;
        public AuthError Error;
    }
}