using System;
using System.Collections.Generic;
using CursedOnion.Game.Events;
using CursedOnion.Game.Logic;
using CursedOnion.Locators;
using NaughtyAttributes;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Objects
{
    public class MapManager : MonoBehaviour
    {
        [Inject] RuntimeVariableLocator variableLocator;
        public MapEvents MapEvents;
        [SerializeField, ReadOnly] List<LevelPlatform> levels = new List<LevelPlatform>();
        
        int selectedLevel;
        public void AddLevel(LevelPlatform level)
        {
            if (!levels.Contains(level))
            {
                levels.Add(level);
                levels.Sort((a, b) => a.LevelInformation.LevelIndex.CompareTo(b.LevelInformation.LevelIndex));
            }
        }
        void Start()
        {
            selectedLevel = variableLocator.LastLevelPlayed;
            levels[selectedLevel].Select();
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
            levels[selectedLevel].Select();
        }
        
    }
}
