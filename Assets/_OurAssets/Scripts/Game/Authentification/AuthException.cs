using System;
using Unity.Services.Authentication;
using Unity.Services.Core;

namespace CursedOnion.Game.Authentication
{
    
    public static class AuthExceptionHandler
    {
        public static int MapExceptionToAuthError(Exception ex)
        {
            if (ex.Message.Contains("not in the correct format")
                || ex.Message.Contains("does not match requirements")
                || ex.Message.Contains("Invalid"))
                return 1;

            if (ex.Message.Contains("username already exists"))
                return 3;

            return -1;
        }
    }
    public struct AuthResult
    {
        public bool Success => Error == 0;
        public int Error;
    }
}