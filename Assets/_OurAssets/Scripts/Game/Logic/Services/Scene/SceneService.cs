using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CursedOnion.Game.Logic.Services
{
    public class SceneService : IService
    {
        private string currentSceneName;
        
        public Action<string> OnSceneLoadComplete;

        public void ChangeScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            if (loadSceneAsync != null)
            {
                loadSceneAsync.completed += (operation) => OnSceneLoadComplete?.Invoke(sceneName);
            }
            else
            {
                Debug.LogError($"Scene not found: {sceneName}");
            }
        }
    }
}
