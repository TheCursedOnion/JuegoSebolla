using System;
using CursedOnion.Game.Inputs.Camera;
using UnityEngine;

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
        
        public event Action<Transform> OnCameraFollow;
        public void OnCameraFollowChanged(Transform newTarget)
        {
            OnCameraFollow?.Invoke(newTarget);
        }
    }
}