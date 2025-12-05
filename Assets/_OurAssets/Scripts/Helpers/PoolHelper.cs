using System;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace CursedOnion.Helpers
{
    public static class PoolHelper
    {
        public static ObjectPool<T> CreatePool<T>(Func<T> createFunc) where T : class
        {
            return new ObjectPool<T>(
                createFunc,
                item =>
                {
                    if (!TrySetActive(item, true))
                    {
                        item = createFunc();
                    }
                },
                item => TrySetActive(item, false),
                item => DestroyObject(item),
                collectionCheck: false,
                defaultCapacity: 10,
                maxSize: 50
            );
        }

        private static bool TrySetActive<T>(T item, bool active)
        {
            if (item == null) return false;
            if (item is Object unityObj && unityObj == null) return false;

            switch (item)
            {
                case GameObject go:
                    go.SetActive(active);
                    return true;

                case Component c:
                    c.gameObject.SetActive(active);
                    return true;

                default:
                    throw new ArgumentException("Type must be GameObject or Component");
            }
        }

        private static void DestroyObject<T>(T item)
        {
            if (item == null) return;
            if (item is Object unityObj && unityObj == null) return;

            switch (item)
            {
                case GameObject go:
                    Object.Destroy(go);
                    break;

                case Component c:
                    Object.Destroy(c.gameObject);
                    break;

                default:
                    throw new ArgumentException("Type must be GameObject or Component");
            }
        }
    }
}