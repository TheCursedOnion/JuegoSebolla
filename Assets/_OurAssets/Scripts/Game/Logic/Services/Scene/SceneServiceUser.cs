using CursedOnion.Locators;
using CursedOnion.Game.Modes.General.UI.Transitions;
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
        public void ResetScene(float duration, float inBetweenTime, TransitionType type, Color color)
        {
            UITransitionData transitionData = new UITransitionData(duration, inBetweenTime, type, color, null, null);
            ChangeScene(gameObject.scene.name, transitionData);
        }
        public void ChangeScene(string sceneName, float duration, float inBetweenTime, TransitionType type, Color color)
        {
            UITransitionData transitionData = new UITransitionData(duration, inBetweenTime, type, color, null, null);
            ChangeScene(sceneName, transitionData);
        }
        public void ChangeScene(string sceneName, UITransitionData uiTransitionData)
        {
            if(uiTransitionData.Type == TransitionType.None)
                _ = sceneService.ChangeScene(sceneName);
            else
            {
                float halfDuration = uiTransitionData.Duration / 2f;
                var transitionLocator = gameObject.scene.GetSceneContainer().Resolve<UITransitionLocator>();
                UITransition transition = transitionLocator.GetTransition(uiTransitionData.Type);

                if (transition != null)
                {
                    transition
                        .SetInBetweenTime(uiTransitionData.InBetweenTime)
                        .SetColor(uiTransitionData.Color)
                        .SetMidAction(async () =>
                            {
                                await sceneService.ChangeScene(sceneName);
                                transition.StartCloseTransition(halfDuration);
                            }
                        )
                        .StartOpenTransition(halfDuration);
                }
            }
        }

        void InvokeOnSceneLoadCalled(string sceneName) => onSceneLoadCalled?.Invoke(sceneName);
        void InvokeOnSceneLoadCompleted(string sceneName) => onSceneLoadCompleted?.Invoke(sceneName);
    }
}