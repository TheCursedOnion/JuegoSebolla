using CursedOnion.Locators;
using CursedOnion.UI;
using Reflex.Attributes;
using Reflex.Extensions;
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
        public void ChangeScene(string sceneName, float transitionDuration, float transitionInBetweenTime, Color transitionColor, TransitionType transitionType)
        {
            if(transitionType == TransitionType.None)
                _ = sceneService.ChangeScene(sceneName);
            else
            {
                float halfDuration = transitionDuration / 2f;
                var transitionLocator = gameObject.scene.GetSceneContainer().Resolve<UITransitionLocator>();
                UITransition transition = transitionLocator.GetTransition(transitionType);

                if (transition != null)
                {
                    transition
                        .SetInBetweenTime(transitionInBetweenTime)
                        .SetColor(transitionColor)
                        .SetMidAction(() =>
                            {
                                sceneService.ChangeScene(sceneName);
                                transition.StartCloseTransition(halfDuration);
                            }
                        )
                        .StartOpenTransition(halfDuration);
                    Debug.Log($"Getting transition {transitionType}");
                }
            }
        }

        void InvokeOnSceneLoadCalled(string sceneName) => onSceneLoadCalled?.Invoke(sceneName);
        void InvokeOnSceneLoadCompleted(string sceneName) => onSceneLoadCompleted?.Invoke(sceneName);
    }
}