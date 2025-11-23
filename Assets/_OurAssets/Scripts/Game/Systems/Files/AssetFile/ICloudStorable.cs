using System.Collections.Generic;
using CursedOnion.Game.Authentication;
using Newtonsoft.Json;
using Unity.Services.CloudSave.Models;
using UnityEngine;

namespace CursedOnion.Game.CloudSave
{
    public interface ICloudStorable
    {
        public void SaveInto(Dictionary<string, object> serializableData);
        public void LoadFrom(Dictionary<string, Item> loadedData);
    }
    
    public static class CloudUtils
    {
        public static bool CanUseCloud()
        {
            return CanAccessInternet()
                  && GameAuthenticator.HasSignedIn;
        }
        public static bool CanAccessInternet()
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }
        
        public static T GetValueFromQuery<T>(this Dictionary<string, Item> query, string key)
        {
            return query.TryGetValue(key, out var value) ? Deserialize<T>(value.Value.GetAsString()) : default;
        }
        public static bool TryGetValueFromQuery<T>(this Dictionary<string, Item> query, string key, out T value)
        {
            if (query.TryGetValue(key, out var item))
            {
                value = Deserialize<T>(item.Value.GetAsString());
                return true;
            }
            value = default;
            return false;
        }
        
        private static T Deserialize<T>(string input)
        {
            if (typeof(T) == typeof(string)) return (T)(object)input;
            return JsonConvert.DeserializeObject<T>(input);
        }
    }
    
}