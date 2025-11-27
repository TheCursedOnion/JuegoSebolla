using UnityEngine;

namespace CursedOnion.Game.Systems.Level.Goal
{
    public class ResistRoundsGoal : LevelGoal
    {
        [SerializeField] protected int roundsCount;
        int rounds = 0;
        protected override void OnEnable()
        {
            LevelEvents.OnRoundPassed += CheckGoal;
            LevelEvents.OnAllEnemiesKilled += OnVictory;
            LevelEvents.OnAllAlliesKilled += OnDefeat;
        }
        protected override void OnDisable()
        {
            LevelEvents.OnBossEnemyKilled -= CheckGoal;
            LevelEvents.OnAllEnemiesKilled -= OnVictory;
            LevelEvents.OnAllAlliesKilled -= OnDefeat;
        }
        
        protected override void CheckGoal()
        {
            rounds++;
            if(rounds - 1 == roundsCount) OnVictory();
        }
    }
}