using System;
using System.Collections.Generic;
using CursedOnion.Game.Logic;
using NaughtyAttributes;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Objects
{
    public class MapManager : MonoBehaviour
    {
        [Inject] private MediatorEvents mediatorEvents;
        [SerializeField, ReadOnly] List<LevelPlatform> levels = new List<LevelPlatform>();
        
        int selectedLevel;
        public void AddLevel(LevelPlatform level)
        {
            if (!levels.Contains(level))
            {
                levels.Add(level);
                levels.Sort((a, b) => a.LevelInformation.LevelID.CompareTo(b.LevelInformation.LevelID));
            }
        }

        private void Awake()
        {
            //TODO: Conocer el último nivel completado/jugado
        }

        void Start()
        {
            mediatorEvents.OnLevelInspectionChanged(levels[selectedLevel]);
        }

        public bool TryGetSelectedLevelScene(out string levelSceneName)
        {
            levelSceneName = levels[selectedLevel].LevelInformation.levelScene;
            return !string.IsNullOrEmpty(levelSceneName) && !string.IsNullOrWhiteSpace(levelSceneName);
        }
        public void MoveToNextLevel()
        {
            MoveLevelIndex(1);
        }

        public void MoveToPreviousLevel()
        {
            MoveLevelIndex(-1);
        }

        void MoveLevelIndex(int moveIndex)
        {
            if (moveIndex == 1 && selectedLevel == levels.Count - 1 || moveIndex == -1 && selectedLevel == 0) return;
            selectedLevel += moveIndex;
            mediatorEvents.OnLevelInspectionChanged(levels[selectedLevel]);
        }
        
    }
}
