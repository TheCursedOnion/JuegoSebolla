using System;
using CursedOnion.Game.Inputs.Camera;

namespace CursedOnion.Game.Events
{
    public class CameraEvents : RuntimeEvents
    {
        public event Action<CameraControlFlag> OnModifyCameraMode;

        public void OnCameraModeModified(CameraControlFlag newMode)
        {
            if (!Enabled) return;
            OnModifyCameraMode?.Invoke(newMode);
        }
    }
}