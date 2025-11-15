using System;
using CursedOnion.Game.Objects;
using UnityEngine;

namespace CursedOnion.Game.Events
{
    public class MapEvents : RuntimeEvents
    {
        public event Action<LevelInformation> OnLevelSelected;
        public void SelectLevel(LevelPlatform newLevel)
        {
            if (!Enabled) return;
            OnLevelSelected?.Invoke(newLevel.LevelInformation);
        }
    }
}
