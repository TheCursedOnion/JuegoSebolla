using Reflex.Attributes;
using UltEvents;
using UnityEngine;

namespace CursedOnion.Game.Logic.Services
{
    public class SceneServiceUser : MonoBehaviour
    {
        [Inject] SceneService sceneService;
        
        [SerializeField] UltEvent<string> onSceneLoadCalled;
        [SerializeField] UltEvent<string> onSceneLoadCompleted;
        void OnEnable()
        {
            sceneService.OnSceneLoadCall += InvokeOnSceneLoadCalled;
            sceneService.OnSceneLoadComplete += InvokeOnSceneLoadCompleted;
        }
        void OnDisable()
        {
            sceneService.OnSceneLoadCall -= InvokeOnSceneLoadCalled;
            sceneService.OnSceneLoadComplete -= InvokeOnSceneLoadCompleted;
        }
        public void ChangeScene(string sceneName)
        {
            _ = sceneService.ChangeScene(sceneName);
        }

        void InvokeOnSceneLoadCalled(string sceneName) => onSceneLoadCalled?.Invoke(sceneName);
        void InvokeOnSceneLoadCompleted(string sceneName) => onSceneLoadCompleted?.Invoke(sceneName);
    }
}