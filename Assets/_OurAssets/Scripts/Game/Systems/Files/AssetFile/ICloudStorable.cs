using System.Threading.Tasks;
using CursedOnion.Game.CloudSave;
using Unity.Services.Core;
using UnityEngine;

namespace CursedOnion.Game.CloudSave
{
    public interface ICloudStorable
    {
        public CloudSaveClient SaveClient {get; set;}
        public Task Save();
        public Task Load();
    }
    
    public static class CloudUtils
    {
        public static bool CanUseCloud()
        {
            return Application.internetReachability != NetworkReachability.NotReachable
                   && UnityServices.State == ServicesInitializationState.Initialized;
        }
    }
    
}