using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CursedOnion.Game.Logic.Services
{
    public class SceneService : IService
    {
        private string currentSceneName;

        public Action<string> OnSceneLoadCall;
        public Action<string> OnSceneLoadComplete;
        
        bool changingScene = false;
        public void ResetScene()
        { 
            if(changingScene) return;
            
            _ = ChangeScene(currentSceneName);
        }
        public async Task ChangeScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            if(changingScene) return;
            Debug.Log($"Changing scene to {sceneName}");
            currentSceneName = sceneName;
            
            changingScene = true;
            bool success = await LoadSceneAsync(sceneName);
            if(!success)
            {
                Debug.LogError($"Scene not found: {sceneName}");
            }
            
            changingScene = false;
        }
        async Task<bool> LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            try
            {
                OnSceneLoadCall?.Invoke(sceneName);
                AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync(sceneName, mode);
                if (loadSceneAsync != null)
                {
                    loadSceneAsync.completed += (operation) => OnSceneLoadComplete?.Invoke(sceneName);
                    await loadSceneAsync;
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                return false;
            }
        }
    }
}
