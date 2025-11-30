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
                item => SetActive(item, true),
                item => SetActive(item, false),
                item => DestroyObject(item),
                collectionCheck: false,
                defaultCapacity: 10,
                maxSize: 50
            );
        }
        private static void SetActive<T>(T item, bool active)
        {
            switch (item)
            {
                case GameObject go:
                    go.SetActive(active);
                    break;

                case Component c:
                    c.gameObject.SetActive(active);
                    break;

                default:
                    throw new ArgumentException("Type must be GameObject or Component");
            }
        }

        private static void DestroyObject<T>(T item)
        {
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