using System;
using System.Collections.Generic;

namespace CursedOnion.Game.Entity.Effects
{
    public static class EntityEffectFactory
    {
        private static Dictionary<Type, EffectData> registry = new ();

        public static void RegisterEffect<T>(EffectData data) where T : StatusEffect
        {
            registry[typeof(T)] = data;
        }

        public static StatusEffect CreateEffect<T>(int customDuration = -1, float customMagnitude = -1f) where T : StatusEffect
        {
            if (!registry.TryGetValue(typeof(T), out var data))
                throw new Exception($"No se ha registrado Data para {typeof(T)}");

            return data.CreateInstance(customDuration, customMagnitude);
        }
    }
}
