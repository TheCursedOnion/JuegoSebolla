using NaughtyAttributes;
using UnityEngine;

namespace CursedOnion.Game.Systems.Level
{
    public enum LevelState { InDialog, InBattleEditor, InBattle, Finished, InResults}
    public enum LevelTimePeriod { Greece = 0, Egypt = 1, Japan = 2}
    
    [System.Serializable]
    public class LevelData
    {
        [Header("Historic Period")]
        [SerializeField] private LevelTimePeriod timePeriod;
        public LevelTimePeriod TimePeriod => timePeriod;
        
        [Header("\nLevel Start Data")]
        [SerializeField] private int startingGold = 1000;
        public int StartingGold => startingGold;
        
        [SerializeField] private LevelState startingState;
        public LevelState StartingState => startingState;
        
        [Header("\nLevel End Data")]
        [SerializeField] private bool levelHasEndDialog;
        public bool LevelHasEndDialog => levelHasEndDialog;
        
        [Header("\nLevel Meta Data")]
        public int LevelIndex;
        [Scene] public string CorrespondingMapSceneName;
        public string LevelBaseKey;
    }
}
