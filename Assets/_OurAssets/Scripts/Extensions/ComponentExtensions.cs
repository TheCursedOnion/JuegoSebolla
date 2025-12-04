using Reflex.Core;
using Reflex.Extensions;
using Reflex.Injectors;
using UnityEngine;

namespace CursedOnion.Extensions
{
    public static class ComponentExtensions
    {
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            T component = go.GetComponent<T>();
            if (component == null)
                component = go.AddComponent<T>();

            return component;
        }
        
        public static void InjectDependencies(this GameObject gameObject)
        {
            var container = gameObject.scene.GetSceneContainer();
            GameObjectInjector.InjectObject(gameObject, container);
        }
        public static void InjectDependencies(this object obj, Container container)
        {
            AttributeInjector.Inject(obj, container);
        }

        public static T GetOrAddComponent<T>(this Component c) where T : Component
        {
            return GetOrAddComponent<T>(c.gameObject);
        }

        public static void SetGroupActive(this CanvasGroup canvasGroup, bool enable, float time, float delay = 0)
        {
            canvasGroup.interactable = enable;
            if(enable) canvasGroup.gameObject.SetActive(true);
            
            LeanTween.cancel(canvasGroup.gameObject);
            
            float final = enable ? 1 : 0;
            LeanTween.alphaCanvas(canvasGroup, final, time).setEase(LeanTweenType.easeInOutQuad)
                .setDelay(delay)
                .setOnComplete(() =>
                {
                    if (!enable) canvasGroup.gameObject.SetActive(false);
                });
        }
    }
}