using UnityEngine;

namespace CursedOnion.Game.Systems.Level
{
    public enum LevelGoal
    {
        DefeatAll,
        DefeatBoss,
        EndureRounds,
    }
    
    [CreateAssetMenu(fileName = "LevelData", menuName = "Game/FileAsset/LevelData")]
    public class LevelData : ScriptableObject
    {
        [SerializeField] private int startingGold;
        public int StartingGold => startingGold;
        
        [SerializeField] private LevelGoal goal;
        public LevelGoal Goal => goal;
    }
}
