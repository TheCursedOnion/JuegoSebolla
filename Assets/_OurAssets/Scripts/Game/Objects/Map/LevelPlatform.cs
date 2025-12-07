using System;
using CursedOnion.Game.Cameras;
using CursedOnion.Game.Events;
using CursedOnion.Locators;
using NaughtyAttributes;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Objects
{
    
    [Serializable]
    public class LevelInformation
    {
        public enum LevelType { Start, Normal, End, Only }
        
        public int LevelIndex;
        public LevelType LevelEnumType;
        
        public string BaseKey;
        
        [Scene] public string levelScene;

        public bool Validate()
        {
            return !string.IsNullOrEmpty(levelScene);
        }
    }
    public class LevelPlatform : MonoBehaviour
    {
        private static readonly int TintColor = Shader.PropertyToID("_Color");
        [Inject] MapManager mapManager;
        [Inject] RuntimeVariableLocator variableLocator;
        MapEvents mapEvents;
        
        [SerializeField] MeshRenderer platformCenterMesh;
        [SerializeField] MeshRenderer platformRingMesh;
        [SerializeField] CameraFocus cameraFocus;
        
        public LevelInformation LevelInformation;

        private void Awake()
        {
            cameraFocus ??= GetComponent<CameraFocus>();
            mapManager.AddLevel(this);
            mapEvents = mapManager.MapEvents;
            
            int levelIndex = LevelInformation.LevelIndex;
            int lastCompletedLevel = variableLocator.LastCompletedLevel;
            
            Color centerColor = levelIndex switch
            {
                _ when levelIndex > lastCompletedLevel + 1 => Color.gray,
                _ when levelIndex == lastCompletedLevel + 1 => Color.red,
                _ => Color.blue
            };
            Color ringColor = levelIndex switch
            {
                _ when levelIndex > lastCompletedLevel + 1 => new Color(0.3f, 0.3f, 0.3f),
                _ => platformRingMesh.material.color
            };

            platformCenterMesh.material.color = centerColor;
            platformRingMesh.material.color = ringColor;
        }

        public void Select()
        {
            if(!IsValid()) return;
            
            cameraFocus.RequestFocus();
            mapEvents.SelectLevel(this);
        }
        
        public bool IsValid()
        {
            return LevelInformation != null && LevelInformation.Validate();
        }
    }
}