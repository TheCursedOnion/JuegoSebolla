using UnityEngine;

namespace CursedOnion.Game.Systems.Level
{
    public enum LevelGoal
    {
        DefeatAll,
        DefeatBoss,
        EndureRounds,
    }
    
    [System.Serializable]
    public class LevelData
    {
        [SerializeField] private int startingGold;
        public int StartingGold => startingGold;
        
        [SerializeField] private LevelGoal goal;
        public LevelGoal Goal => goal;
        
        [SerializeField] private LevelState startingState;
        public LevelState StartingState => startingState;
    }
}
