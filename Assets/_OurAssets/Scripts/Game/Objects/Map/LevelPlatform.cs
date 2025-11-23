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
        public enum LevelType { Start, Normal, End }
        
        public int LevelIndex;
        public LevelType LevelEnumType;
        
        public string NameKey;
        public string DescriptionKey;
        
        [Scene] public string levelScene;

        public bool Validate()
        {
            return !string.IsNullOrEmpty(levelScene);
        }
    }
    public class LevelPlatform : MonoBehaviour
    {
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
                _ when levelIndex == lastCompletedLevel + 1 => Color.blue,
                _ => Color.red
            };
            Color ringColor = levelIndex switch
            {
                _ when levelIndex > lastCompletedLevel + 1 => Color.gray,
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