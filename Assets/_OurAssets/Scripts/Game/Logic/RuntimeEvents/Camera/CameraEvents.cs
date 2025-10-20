using System;
using CursedOnion.Behaviours;

namespace CursedOnion.Game.Events
{
    public class CameraEvents : RuntimeEvents
    {
        public CameraEvents(bool startEnabled) : base(startEnabled)
        {
        }

        public event Action<CameraMode> OnModifyCameraMode;

        public void OnCameraModeModified(CameraMode newMode)
        {
            if (!Enabled) return;
            OnModifyCameraMode?.Invoke(newMode);
        }
    }
}