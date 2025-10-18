using System;
using CursedOnion.Behaviours;
using CursedOnion.Game.Objects;
using UnityEngine;

namespace CursedOnion.Game.Logic
{
    [CreateAssetMenu(fileName = "MediatorEvents", menuName = "Game/MediatorEvents")]
    public class MediatorEvents : ScriptableObject
    {
        public event Action<CameraMode> OnModifyCameraMode;
        public event Action<LevelPlatform> OnLevelInspectionChange;
        public void OnCameraModeModified(CameraMode newMode) => OnModifyCameraMode?.Invoke(newMode);
        public void OnLevelInspectionChanged(LevelPlatform newLevel) => OnLevelInspectionChange?.Invoke(newLevel);
    }
}
