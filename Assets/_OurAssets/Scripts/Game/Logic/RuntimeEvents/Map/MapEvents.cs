using System;
using CursedOnion.Game.Objects;
using UnityEngine;

namespace CursedOnion.Game.Events
{
    public class MapEvents : RuntimeEvents
    {
        public event Action<LevelPlatform> OnLevelPlatformChange;
        public void OnLevelPlatformChanged(LevelPlatform newLevel)
        {
            if (!Enabled) return;
            OnLevelPlatformChange?.Invoke(newLevel);
        }
    }
}
