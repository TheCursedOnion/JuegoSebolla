using UnityEngine;

namespace CursedOnion.Game.Systems.Level
{
    public enum LevelGoal
    {
        DefeatAll,
        DefeatBoss,
        EndureRounds,
    }
    public enum LevelState { InDialog, InBattleEditor, InBattle, Finished }
    public enum LevelTimePeriod { Greece = 0, Egypt = 1, Italy = 2}
    
    [System.Serializable]
    public class LevelData
    {
        [Header("Historic Period")]
        [SerializeField] private LevelTimePeriod timePeriod;
        public LevelTimePeriod TimePeriod => timePeriod;
        
        [Header("\nLevel Start Data")]
        [SerializeField] private int startingGold;
        public int StartingGold => startingGold;
        
        [SerializeField] private LevelState startingState;
        public LevelState StartingState => startingState;
        
        [Header("\nLevel Goal")]
        [SerializeField] private LevelGoal goal;
        public LevelGoal Goal => goal;
    }
}
