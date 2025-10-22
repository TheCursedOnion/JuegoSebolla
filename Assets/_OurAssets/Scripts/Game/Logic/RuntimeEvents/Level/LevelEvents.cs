using System;
using CursedOnion.Game.Entity;
using UnityEngine;

namespace CursedOnion.Game.Events
{
    public class LevelEvents : RuntimeEvents
    {
        public LevelEvents(bool startEnabled) : base(startEnabled)
        {
        }

        public event Action<Entity.SimpleEntity> OnEntityInspected;
        public void OnEntityInspection(Entity.SimpleEntity inspectedEntity)
        {
            if (!Enabled) return;
            OnEntityInspected?.Invoke(inspectedEntity);
        }
    }
}
