using System;
using System.Collections.Generic;

namespace CursedOnion.Helpers
{
    public static class FlagHelper
    {
        public static T Raise<T>(this T current, T flag) where T : Enum
        {
            int curr = Convert.ToInt32(current);
            int f = Convert.ToInt32(flag);
            return (T)Enum.ToObject(typeof(T), curr | f);
        }
        public static bool HasRaised<T>(this T current, T flag) where T : Enum
        {
            int curr = Convert.ToInt32(current);
            int f = Convert.ToInt32(flag);
            return (curr & f) != 0;
        }
        public static bool HasRaisedNone<T>(this T current) where T : Enum
        {
            int curr = Convert.ToInt32(current);
            return curr == 0;
        }
        public static T Reset<T>(this T current, T flag) where T : Enum
        {
            int curr = Convert.ToInt32(current);
            int f = Convert.ToInt32(flag);
            return (T)Enum.ToObject(typeof(T), curr & ~f);
        }
        
        public static IEnumerable<T> GetActiveFlags<T>(this T value) where T : Enum
        {
            foreach (T flag in Enum.GetValues(typeof(T)))
            {
                int f = Convert.ToInt32(flag);
                int v = Convert.ToInt32(value);

                if (f != 0 && (v & f) == f)
                    yield return flag;
            }
        }
    }
}