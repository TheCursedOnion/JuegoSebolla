using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Internal;
using Unity.Services.CloudSave.Models;
using UnityEngine;

namespace CursedOnion.Game.CloudSave
{
    public class CloudSaveClient : ISaveClient
    {
        private readonly IDataService client = CloudSaveService.Instance.Data;

        public async Task Save(string key, object value)
        {
            var data = new Dictionary<string, object> { { key, JsonConvert.SerializeObject(value) } };
            await Call(client.Player.SaveAsync(data));
        }
        public async Task Save(params (string key, object value)[] values)
        {
            var data = values.ToDictionary(item => item.key, item => item.value);
            await Call(client.Player.SaveAsync(data));
        }
        public async Task Save(Dictionary<string, object> data)
        {
            if(data.Count == 0) return;
            await Call(client.Player.SaveAsync(data));
        }

        public async Task<T> Load<T>(string key)
        {
            var query = await Call(client.Player.LoadAsync(new HashSet<string> { key }));
            return query.GetValueFromQuery<T>(key);
        }

        public async Task<IEnumerable<T>> Load<T>(params string[] keys)
        {
            var query = await Call(client.Player.LoadAsync(keys.ToHashSet()));

            return keys.Select(k => query.GetValueFromQuery<T>(k));
        }
        
        public async Task<Dictionary<string, Item>> LoadAll()
        {
            return await Call(client.Player.LoadAllAsync());
        }

        public async Task Delete(string key)
        {
            var options = new Unity.Services.CloudSave.Models.Data.Player.DeleteOptions();
            await Call(client.Player.DeleteAsync(key, options));
        }

        public async Task DeleteAll()
        {
            var data = await Call(client.Player.LoadAllAsync());

            var options = new Unity.Services.CloudSave.Models.Data.Player.DeleteOptions();
            
            List<Task> tasks = new List<Task>();
            foreach (var key in data.Keys)
            {
                tasks.Add(client.Player.DeleteAsync(key, options));
            }
            await Call(Task.WhenAll(tasks));
        }

        private static async Task Call(Task action)
        {
            try
            {
                await action;
            }
            catch (CloudSaveValidationException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveRateLimitedException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveException e)
            {
                Debug.LogError($"CloudSave error: {e.Message}");
                if (e.Message.Contains("409"))
                {
                    Debug.LogWarning("Conflicto detectado, reintentando con sobrescritura...");
                    //await CloudSaveService.Instance.Data.ForceSaveAsync(data);
                }
            }
        }

        private static async Task<T> Call<T>(Task<T> action)
        {
            try
            {
                return await action;
            }
            catch (CloudSaveValidationException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveRateLimitedException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveException e)
            {
                Debug.LogError(e);
            }

            return default;
        }
    }

    public interface ISaveClient
    {
        Task Save(string key, object value);

        Task Save(params (string key, object value)[] values);
        
        Task Save(Dictionary<string, object> values);

        Task<T> Load<T>(string key);

        Task<IEnumerable<T>> Load<T>(params string[] keys);

        Task Delete(string key);

        Task DeleteAll();
    }
}
