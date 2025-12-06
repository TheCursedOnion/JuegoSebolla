using System;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace CursedOnion.Helpers
{
    public static class PoolHelper
    {
        public static ObjectPool<T> CreatePool<T>(Func<T> createFunc, int maxSize = 50) where T : UnityEngine.Object
        {
            return new ObjectPool<T>(
                createFunc,
                actionOnGet: item =>
                {
                    if (!IsValid(item)) return;
                    SetActive(item, true);
                },
                actionOnRelease: item =>
                {
                    if (!IsValid(item)) return;
                    SetActive(item, false);
                },
                actionOnDestroy: item =>
                {
                    if (IsValid(item))
                        UnityEngine.Object.Destroy(item);
                },
                collectionCheck: false,
                defaultCapacity: 10,
                maxSize: maxSize
            );
        }

        private static bool IsValid(UnityEngine.Object obj)
        {
            return obj != null;
        }

        private static void SetActive(UnityEngine.Object obj, bool active)
        {
            switch (obj)
            {
                case GameObject go:
                    go.SetActive(active);
                    break;

                case Component comp:
                    comp.gameObject.SetActive(active);
                    break;
            }
        }
    }
}