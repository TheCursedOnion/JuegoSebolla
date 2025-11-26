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

        public static T GetOrAddComponent<T>(this Component c) where T : Component
        {
            return GetOrAddComponent<T>(c.gameObject);
        }
    }
}