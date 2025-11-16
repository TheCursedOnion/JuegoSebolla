using System;
using CursedOnion.Game.Cameras;
using CursedOnion.Game.Events;
using NaughtyAttributes;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Objects
{
    
    [Serializable]
    public class LevelInformation
    {
        public enum LevelType { Start, Normal, End }
        
        public int LevelIndex;
        public LevelType LevelEnumType;
        
        public string NameKey;
        
        [Scene] public string levelScene;

        public bool Validate()
        {
            return !string.IsNullOrEmpty(levelScene);
        }
    }
    public class LevelPlatform : MonoBehaviour
    {
        [Inject] MapManager mapManager;
        MapEvents mapEvents;
        [SerializeField] CameraFocus cameraFocus;
        
        public LevelInformation LevelInformation;

        private void Awake()
        {
            cameraFocus ??= GetComponent<CameraFocus>();
            mapManager.AddLevel(this);
            mapEvents = mapManager.MapEvents;
        }

        public void Select()
        {
            if(!IsValid()) return;
            
            cameraFocus.Focus();
            mapEvents.SelectLevel(this);
        }
        
        public bool IsValid()
        {
            return LevelInformation != null && LevelInformation.Validate();
        }
    }
}