using System;
using CursedOnion.Game.Objects;
using UnityEngine;

namespace CursedOnion.Game.Events
{
    public class MapEvents : RuntimeEvents
    {
        public MapEvents(bool startEnabled) : base(startEnabled)
        {
        }

        public event Action<LevelPlatform> OnLevelPlatformChange;
        public void OnLevelPlatformChanged(LevelPlatform newLevel)
        {
            if (!Enabled) return;
            OnLevelPlatformChange?.Invoke(newLevel);
        }
    }
}
