using UnityEngine;

namespace CursedOnion.Game.Systems.Level.Goal
{
    public class KillBossEnemiesGoal : LevelGoal
    {
        [SerializeField] protected int bossEnemiesCount;
        int bossEnemiesKillCount = 0;
        protected override void OnEnable()
        {
            LevelEvents.OnBossEnemyKilled += CheckGoal;
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
            bossEnemiesKillCount++;
            if(bossEnemiesKillCount == bossEnemiesCount) OnVictory();
        }
    }
}