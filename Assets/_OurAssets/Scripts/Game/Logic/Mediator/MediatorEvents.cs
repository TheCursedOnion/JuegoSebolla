using System;
using CursedOnion.Behaviours;
using UnityEngine;

namespace CursedOnion.Game.Logic
{
    [CreateAssetMenu(fileName = "MediatorEvents", menuName = "Game/MediatorEvents")]
    public class MediatorEvents : ScriptableObject
    {
        public event Action<CameraMode> OnModifyCameraMode;
        
        public void OnCameraModeModified(CameraMode newMode) => OnModifyCameraMode?.Invoke(newMode);
    }
}
