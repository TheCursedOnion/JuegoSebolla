using System;
using NaughtyAttributes;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Objects
{
    [System.Serializable]
    public class LevelInformation
    {
        public int LevelID;
        
        public string Name;
        public string Description;
        
        [Scene] public string levelScene;
    }
    public class LevelPlatform : MonoBehaviour
    {
        [Inject] MapManager mapManager;
        
        public LevelInformation LevelInformation;

        private void Awake()
        {
            mapManager.AddLevel(this);
        }
    }
}